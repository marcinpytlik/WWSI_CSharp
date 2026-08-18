namespace Demo11;

public sealed class OrderProcessor
{
    private readonly IOrderStore _store;

    public OrderProcessor(IOrderStore store) => _store = store;

    public async Task HandleAsync(OrderPlaced message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (string.IsNullOrWhiteSpace(message.Sku))
            throw new ArgumentException("SKU is required.", nameof(message));
        if (message.Qty <= 0)
            throw new ArgumentOutOfRangeException(nameof(message), "Qty must be positive.");

        await _store.AddAsync(new OrderRecord
        {
            Id = message.OrderId == Guid.Empty ? Guid.NewGuid() : message.OrderId,
            Sku = message.Sku.Trim(),
            Qty = message.Qty,
            ReceivedUtc = DateTime.UtcNow
        }, cancellationToken);
    }
}
