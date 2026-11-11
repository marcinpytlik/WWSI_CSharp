using System.Diagnostics;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using QueueCacheDemo.Contracts;
using QueueCacheDemo.Data;
using QueueCacheDemo.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Sql"), sql => sql.EnableRetryOnFailure()));
builder.Services.AddStackExchangeRedisCache(o =>
    o.Configuration = builder.Configuration.GetConnectionString("Redis")!);
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddSource("Wwsi.QueueCache");
        if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
            tracing.AddOtlpExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics.AddMeter("Wwsi.QueueCache");
        if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
            metrics.AddOtlpExporter();
    });
builder.Services.AddHostedService<Consumer>();
builder.Services.AddHostedService<OutboxPublisher>();
await builder.Build().RunAsync();

internal static class Messaging
{
    public static ConnectionFactory Factory(IConfiguration config) => new()
    {
        HostName = config["Rabbit:Host"] ?? "localhost",
        UserName = config["Rabbit:User"] ?? throw new InvalidOperationException("Rabbit:User required"),
        Password = config["Rabbit:Password"] ?? throw new InvalidOperationException("Rabbit:Password required")
    };

    public static async Task DeclareAsync(IChannel channel, CancellationToken ct)
    {
        await channel.ExchangeDeclareAsync("products.dlx", ExchangeType.Direct, durable: true, cancellationToken: ct);
        await channel.QueueDeclareAsync("products.dead", durable: true, exclusive: false, autoDelete: false, cancellationToken: ct);
        await channel.QueueBindAsync("products.dead", "products.dlx", "products.create", cancellationToken: ct);
        await channel.QueueDeclareAsync("products.create", durable: true, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object?> { ["x-dead-letter-exchange"] = "products.dlx" },
            cancellationToken: ct);
        await channel.QueueDeclareAsync("products.retry", durable: true, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-message-ttl"] = 5000,
                ["x-dead-letter-exchange"] = "",
                ["x-dead-letter-routing-key"] = "products.create"
            }, cancellationToken: ct);
    }
}

internal static class DemoTelemetry
{
    public static readonly ActivitySource Activity = new("Wwsi.QueueCache");
    private static readonly Meter Meter = new("Wwsi.QueueCache");
    public static readonly Counter<long> Completed = Meter.CreateCounter<long>("wwsi.commands.completed");
    public static readonly Counter<long> Retried = Meter.CreateCounter<long>("wwsi.commands.retried");
    public static readonly Counter<long> DeadLettered = Meter.CreateCounter<long>("wwsi.commands.deadlettered");
}

public sealed class OutboxPublisher(
    IServiceScopeFactory scopes, IConfiguration config, ILogger<OutboxPublisher> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // Reconnect when RabbitMQ fails/restarts. Publishing is at-least-once.
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await using var connection = await Messaging.Factory(config).CreateConnectionAsync(ct);
                await using var channel = await connection.CreateChannelAsync(
                    new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true), ct);
                await Messaging.DeclareAsync(channel, ct);
                var leaseOwner = Guid.NewGuid();
                while (!ct.IsCancellationRequested)
                {
                    using var scope = scopes.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var now = DateTime.UtcNow;
                    var pending = await db.OutboxMessages.AsNoTracking()
                        .Where(x => x.PublishedAtUtc == null &&
                                    (x.LeaseUntilUtc == null || x.LeaseUntilUtc < now))
                        .OrderBy(x => x.CreatedAtUtc).Take(20).ToListAsync(ct);
                    foreach (var row in pending)
                    {
                        var claimed = await db.OutboxMessages
                            .Where(x => x.Id == row.Id && x.PublishedAtUtc == null &&
                                        (x.LeaseUntilUtc == null || x.LeaseUntilUtc < now))
                            .ExecuteUpdateAsync(s => s
                                .SetProperty(x => x.LeaseOwner, leaseOwner)
                                .SetProperty(x => x.LeaseUntilUtc, now.AddMinutes(1)), ct);
                        if (claimed == 0) continue;
                        await channel.BasicPublishAsync("", "products.create", mandatory: true,
                            basicProperties: new BasicProperties { Persistent = true, MessageId = row.OperationId.ToString() },
                            body: Encoding.UTF8.GetBytes(row.Payload), cancellationToken: ct);

                        // If this UPDATE fails, re-publishing is safe: consumer is idempotent.
                        await db.OutboxMessages.Where(x => x.Id == row.Id &&
                                    x.LeaseOwner == leaseOwner && x.PublishedAtUtc == null)
                            .ExecuteUpdateAsync(s => s
                                .SetProperty(x => x.PublishedAtUtc, DateTime.UtcNow)
                                .SetProperty(x => x.LeaseOwner, (Guid?)null)
                                .SetProperty(x => x.LeaseUntilUtc, (DateTime?)null), ct);
                        logger.LogInformation("Published outbox {OutboxId} operation {OperationId}",
                            row.Id, row.OperationId);
                    }
                    await Task.Delay(pending.Count == 0 ? 1000 : 100, ct);
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested) { break; }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox publisher disconnected; reconnecting");
                await Task.Delay(TimeSpan.FromSeconds(3), ct);
            }
        }
    }
}

public sealed class Consumer(
    IServiceScopeFactory scopes, IConfiguration config, ILogger<Consumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await using var connection = await Messaging.Factory(config).CreateConnectionAsync(ct);
                await using var channel = await connection.CreateChannelAsync(
                    new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true), ct);
                await Messaging.DeclareAsync(channel, ct);
                await channel.BasicQosAsync(0, 1, false, ct);
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (_, ea) => await HandleAsync(channel, ea, ct);
                await channel.BasicConsumeAsync("products.create", autoAck: false, consumer: consumer, cancellationToken: ct);
                logger.LogInformation("RabbitMQ consumer started");
                await Task.Delay(Timeout.InfiniteTimeSpan, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested) { break; }
            catch (Exception ex)
            {
                logger.LogError(ex, "Consumer connection failed; reconnecting");
                await Task.Delay(TimeSpan.FromSeconds(3), ct);
            }
        }
    }

    private async Task HandleAsync(IChannel channel, BasicDeliverEventArgs ea, CancellationToken ct)
    {
        Guid? operationId = null;
        try
        {
            var command = JsonSerializer.Deserialize<CreateProduct>(ea.Body.Span)
                          ?? throw new InvalidOperationException("Empty command");
            operationId = command.OperationId;
            using var activity = DemoTelemetry.Activity.StartActivity("HandleCreateProduct",
                ActivityKind.Consumer, command.TraceParent);

            using var strategyScope = scopes.CreateScope();
            var strategyDb = strategyScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var strategy = strategyDb.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var txScope = scopes.CreateScope();
                var db = txScope.ServiceProvider.GetRequiredService<AppDbContext>();
                await using var tx = await db.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable, ct);
                if (!await db.ProcessedCommands.AnyAsync(x => x.Id == command.OperationId, ct))
                {
                    var operation = await db.OperationStatuses
                        .SingleAsync(x => x.Id == command.OperationId, ct);
                    if (!string.Equals(operation.PrincipalId, command.PrincipalId, StringComparison.Ordinal))
                        throw new InvalidOperationException("Principal mismatch");
                    db.Products.Add(new Product
                    {
                        Id = command.ProductId,
                        PrincipalId = command.PrincipalId,
                        Name = command.Name,
                        Price = command.Price,
                        CreatedAtUtc = DateTime.UtcNow
                    });
                    db.ProcessedCommands.Add(new ProcessedCommand
                    {
                        Id = command.OperationId, CompletedAtUtc = DateTime.UtcNow
                    });
                    operation.Status = "Completed";
                    operation.ErrorCode = null;
                    operation.UpdatedAtUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync(ct);
                }
                await tx.CommitAsync(ct);
            });

            // Cache is best effort, SQL stays the system of record.
            using (var scope = scopes.CreateScope())
            {
                var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
                try { await cache.RemoveAsync($"products:{command.ProductId:N}:{command.PrincipalId}", ct); }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogWarning(ex, "Redis invalidation failed: {ProductId}", command.ProductId);
                }
            }

            await channel.BasicAckAsync(ea.DeliveryTag, false, ct);
            DemoTelemetry.Completed.Add(1);
            logger.LogInformation("Completed {OperationId}", command.OperationId);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception ex)
        {
            var attempts = 0;
            if (ea.BasicProperties.Headers?.TryGetValue("x-demo-attempt", out var value) == true)
            {
                try { attempts = Convert.ToInt32(value); }
                catch (Exception) { attempts = 0; }
            }

            logger.LogError(ex, "Processing {OperationId} failed attempt {Attempt}", operationId, attempts + 1);
            if (attempts < 3)
            {
                try
                {
                    await channel.BasicPublishAsync("", "products.retry", mandatory: true,
                        basicProperties: new BasicProperties
                        {
                            Persistent = true,
                            MessageId = operationId?.ToString(),
                            Headers = new Dictionary<string, object?> { ["x-demo-attempt"] = attempts + 1 }
                        },
                        body: ea.Body, cancellationToken: ct);
                    await channel.BasicAckAsync(ea.DeliveryTag, false, ct);
                    DemoTelemetry.Retried.Add(1);
                    return;
                }
                catch (Exception publishError)
                {
                    logger.LogError(publishError, "Retry publication failed; original message remains unacked");
                    throw;
                }
            }

            // Mark failure when SQL is reachable; do not change an already completed operation.
            if (operationId.HasValue)
            {
                try
                {
                    using var scope = scopes.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    await db.OperationStatuses
                        .Where(x => x.Id == operationId.Value && x.Status != "Completed")
                        .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, "Failed")
                            .SetProperty(x => x.ErrorCode, "PROCESSING_FAILED")
                            .SetProperty(x => x.UpdatedAtUtc, DateTime.UtcNow), ct);
                }
                catch (Exception updateError)
                {
                    logger.LogError(updateError, "Failed to persist operation failure status");
                }
            }
            await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false, cancellationToken: ct);
            DemoTelemetry.DeadLettered.Add(1);
        }
    }
}
