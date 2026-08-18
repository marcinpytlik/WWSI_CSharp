using Demo11;
using Xunit;

namespace Demo11.Tests;

public sealed class InMemoryOrderStore : IOrderStore
{
    private readonly Dictionary<Guid, OrderRecord> _db = new();

    public Task AddAsync(OrderRecord order, CancellationToken cancellationToken = default)
    {
        _db[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task<OrderRecord?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_db.TryGetValue(id, out var row) ? row : null);
}

public class OrderProcessorTests
{
    [Fact]
    public async Task HandleAsync_SavesOrder()
    {
        var store = new InMemoryOrderStore();
        var processor = new OrderProcessor(store);
        var id = Guid.NewGuid();

        await processor.HandleAsync(new OrderPlaced(id, "SKU-1", 3));

        var saved = await store.GetAsync(id);
        Assert.NotNull(saved);
        Assert.Equal("SKU-1", saved!.Sku);
        Assert.Equal(3, saved.Qty);
    }

    [Fact]
    public async Task HandleAsync_RejectsNonPositiveQty()
    {
        var processor = new OrderProcessor(new InMemoryOrderStore());
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => processor.HandleAsync(new OrderPlaced(Guid.NewGuid(), "SKU-1", 0)));
    }

    [Fact]
    public async Task HandleAsync_RejectsEmptySku()
    {
        var processor = new OrderProcessor(new InMemoryOrderStore());
        await Assert.ThrowsAsync<ArgumentException>(
            () => processor.HandleAsync(new OrderPlaced(Guid.NewGuid(), "  ", 1)));
    }
}
