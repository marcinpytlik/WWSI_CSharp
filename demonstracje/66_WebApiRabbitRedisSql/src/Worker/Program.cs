using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using QueueCacheDemo.Contracts;
using QueueCacheDemo.Data;
using QueueCacheDemo.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Sql"),
        sql => sql.EnableRetryOnFailure()));
builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("Redis")!);
builder.Services.AddHostedService<Consumer>();

await builder.Build().RunAsync();

public sealed class Consumer(
    IServiceScopeFactory scopes,
    IConfiguration configuration,
    ILogger<Consumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Schema creation and security provisioning belong to db-init, never to the Worker.
        var factory = new ConnectionFactory
        {
            HostName = configuration["Rabbit:Host"] ?? "localhost",
            UserName = configuration["Rabbit:User"] ?? throw new InvalidOperationException("Rabbit:User not configured"),
            Password = configuration["Rabbit:Password"] ?? throw new InvalidOperationException("Rabbit:Password not configured")
        };

        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await channel.ExchangeDeclareAsync("products.dlx", ExchangeType.Direct, durable: true,
            cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync("products.dead", durable: true, exclusive: false, autoDelete: false,
            cancellationToken: stoppingToken);
        await channel.QueueBindAsync("products.dead", "products.dlx", "products.create",
            cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync("products.create", durable: true, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object?> { ["x-dead-letter-exchange"] = "products.dlx" },
            cancellationToken: stoppingToken);
        await channel.BasicQosAsync(0, 1, false, stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, args) =>
        {
            Guid? operationId = null;
            try
            {
                var command = JsonSerializer.Deserialize<CreateProduct>(args.Body.Span)
                              ?? throw new InvalidOperationException("Empty command");
                operationId = command.OperationId;

                using (var strategyScope = scopes.CreateScope())
                {
                    var strategyDb = strategyScope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var strategy = strategyDb.Database.CreateExecutionStrategy();

                    await strategy.ExecuteAsync(async () =>
                    {
                        // Fresh context per retry attempt avoids reuse of tracked entities after failure.
                        using var transactionScope = scopes.CreateScope();
                        var db = transactionScope.ServiceProvider.GetRequiredService<AppDbContext>();
                        await using var tx = await db.Database.BeginTransactionAsync(stoppingToken);

                        if (!await db.ProcessedCommands.AnyAsync(
                                item => item.Id == command.OperationId, stoppingToken))
                        {
                            db.Products.Add(new Product
                            {
                                Id = command.ProductId,
                                Name = command.Name,
                                Price = command.Price,
                                CreatedAtUtc = DateTime.UtcNow
                            });
                            db.ProcessedCommands.Add(new ProcessedCommand
                            {
                                Id = command.OperationId,
                                CompletedAtUtc = DateTime.UtcNow
                            });
                            await db.SaveChangesAsync(stoppingToken);
                        }

                        await tx.CommitAsync(stoppingToken);
                    });
                }

                using (var scope = scopes.CreateScope())
                {
                    var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
                    try
                    {
                        await cache.RemoveAsync($"products:{command.ProductId:N}", stoppingToken);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        logger.LogWarning(ex,
                            "Cache invalidation failed for {ProductId}; TTL limits staleness",
                            command.ProductId);
                    }
                }

                await channel.BasicAckAsync(args.DeliveryTag, multiple: false, stoppingToken);
                logger.LogInformation("Processed command {OperationId} for product {ProductId}",
                    command.OperationId, command.ProductId);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Command {OperationId} failed; routing to dead-letter queue", operationId);
                await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false, stoppingToken);
            }
        };

        await channel.BasicConsumeAsync("products.create", autoAck: false, consumer: consumer,
            cancellationToken: stoppingToken);
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }
}
