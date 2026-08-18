namespace Demo24;

public sealed class OutboxMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Type { get; init; }
    public required string Payload { get; init; }
    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
    public DateTime? ProcessedUtc { get; set; }
}

public interface IOutboxStore
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutboxMessage>> PendingAsync(CancellationToken cancellationToken = default);
    Task MarkProcessedAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IMessageBus
{
    Task PublishAsync(string type, string payload, CancellationToken cancellationToken = default);
}

public sealed class MemoryOutboxStore : IOutboxStore
{
    private readonly List<OutboxMessage> _items = [];

    public Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        _items.Add(message);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<OutboxMessage>> PendingAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<OutboxMessage>>(_items.Where(x => x.ProcessedUtc is null).ToList());

    public Task MarkProcessedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = _items.Single(x => x.Id == id);
        item.ProcessedUtc = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}

public sealed class OutboxProcessor
{
    private readonly IOutboxStore _store;
    private readonly IMessageBus _bus;

    public OutboxProcessor(IOutboxStore store, IMessageBus bus)
    {
        _store = store;
        _bus = bus;
    }

    public async Task PlaceOrderAsync(string sku, int qty, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        await _store.AddAsync(new OutboxMessage
        {
            Type = "OrderPlaced",
            Payload = $"{sku}:{qty}"
        }, cancellationToken);
    }

    public async Task<int> FlushAsync(CancellationToken cancellationToken = default)
    {
        var pending = await _store.PendingAsync(cancellationToken);
        var sent = 0;
        foreach (var message in pending)
        {
            await _bus.PublishAsync(message.Type, message.Payload, cancellationToken);
            await _store.MarkProcessedAsync(message.Id, cancellationToken);
            sent++;
        }
        return sent;
    }
}

public static class Program
{
    public static async Task<int> Main()
    {
        var store = new MemoryOutboxStore();
        var processor = new OutboxProcessor(store, new ConsoleBus());
        await processor.PlaceOrderAsync("SKU-1", 2);
        Console.WriteLine($"flushed={await processor.FlushAsync()}");
        return 0;
    }
}

file sealed class ConsoleBus : IMessageBus
{
    public Task PublishAsync(string type, string payload, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"{type} {payload}");
        return Task.CompletedTask;
    }
}
