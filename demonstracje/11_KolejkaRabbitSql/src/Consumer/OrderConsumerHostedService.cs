using Microsoft.Extensions.Options;

namespace Demo11.Consumer;

public sealed class OrderConsumerHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly RabbitOptions _options;
    private readonly ILogger<OrderConsumerHostedService> _logger;

    public OrderConsumerHostedService(
        IServiceScopeFactory scopes,
        IOptions<RabbitOptions> options,
        ILogger<OrderConsumerHostedService> logger)
    {
        _scopes = scopes;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var consumer = new RabbitConsumer(_options, HandleAsync);
        await StartWithRetryAsync(consumer, stoppingToken);
        _logger.LogInformation("Listening on queue {Queue}. Ctrl+C to stop.", _options.Queue);
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // shutdown
        }
    }

    private async Task StartWithRetryAsync(RabbitConsumer consumer, CancellationToken stoppingToken)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await consumer.StartAsync(stoppingToken);
                return;
            }
            catch (Exception ex) when (attempt < 20 && !stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning(ex, "RabbitMQ niegotowe (próba {Attempt}/20). Ponawiam za 3 s…", attempt);
                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            }
        }
    }

    private async Task HandleAsync(OrderPlaced message, CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = _scopes.CreateAsyncScope();
            var processor = scope.ServiceProvider.GetRequiredService<OrderProcessor>();
            await processor.HandleAsync(message, cancellationToken);
            _logger.LogInformation("Zapisano zamówienie {OrderId} {Sku} x{Qty}", message.OrderId, message.Sku, message.Qty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Nie udało się zapisać zamówienia {OrderId}", message.OrderId);
            throw;
        }
    }
}
