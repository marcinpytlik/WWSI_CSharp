using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using QueueCacheDemo;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Sql"), sql => sql.EnableRetryOnFailure()));
builder.Services.AddStackExchangeRedisCache(o => o.Configuration = builder.Configuration.GetConnectionString("Redis")!);
builder.Services.AddHostedService<Consumer>();
await builder.Build().RunAsync();

public sealed class Consumer(IServiceScopeFactory scopes, IConfiguration config, ILogger<Consumer> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Worker owns demo schema creation. Waits for database initialization.
        using (var scope = scopes.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureCreatedAsync(stoppingToken);
        }
        var factory = new ConnectionFactory
        {
            HostName = config["Rabbit:Host"] ?? "localhost", UserName = "demo", Password = "demo"
        };
        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await channel.ExchangeDeclareAsync("products.dlx", ExchangeType.Direct, durable: true, cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync("products.dead", durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await channel.QueueBindAsync("products.dead", "products.dlx", "products.create", cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync("products.create", durable: true, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object?> { ["x-dead-letter-exchange"] = "products.dlx" },
            cancellationToken: stoppingToken);
        await channel.BasicQosAsync(0, 1, false, stoppingToken);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var cmd = JsonSerializer.Deserialize<CreateProduct>(ea.Body.Span)
                          ?? throw new InvalidOperationException("Empty command");
                using var scope = scopes.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
                // PK deduplicates delivery; transaction protects both product and processed-marker.
                await using (var tx = await db.Database.BeginTransactionAsync(stoppingToken))
                {
                    var processed = await db.ProcessedCommands.AnyAsync(x => x.Id == cmd.OperationId, stoppingToken);
                    if (!processed)
                    {
                        db.Products.Add(new Product
                        {
                            Id = cmd.ProductId, Name = cmd.Name, Price = cmd.Price,
                            CreatedAtUtc = DateTime.UtcNow
                        });
                        db.ProcessedCommands.Add(new ProcessedCommand
                        {
                            Id = cmd.OperationId, CompletedAtUtc = DateTime.UtcNow
                        });
                        await db.SaveChangesAsync(stoppingToken);
                    }
                    await tx.CommitAsync(stoppingToken);
                }
                try { await cache.RemoveAsync($"products:{cmd.ProductId:N}", stoppingToken); }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    log.LogWarning(ex, "Redis invalidation failed; TTL bounds staleness");
                }
                await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                log.LogInformation("Completed {OperationId}", cmd.OperationId);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                log.LogError(ex, "Processing failed: message routed to dead-letter queue");
                await channel.BasicNackAsync(ea.DeliveryTag, false, false, stoppingToken);
            }
        };
        await channel.BasicConsumeAsync("products.create", autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }
}
