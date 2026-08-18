using Demo24;
using Xunit;

namespace Demo24.Tests;

public sealed class RecordingBus : IMessageBus
{
    public List<(string Type, string Payload)> Published { get; } = [];
    public bool Fail { get; set; }

    public Task PublishAsync(string type, string payload, CancellationToken cancellationToken = default)
    {
        if (Fail) throw new InvalidOperationException("bus down");
        Published.Add((type, payload));
        return Task.CompletedTask;
    }
}

public class OutboxTests
{
    [Fact]
    public async Task Flush_Publishes_AndMarksProcessed()
    {
        var store = new MemoryOutboxStore();
        var bus = new RecordingBus();
        var processor = new OutboxProcessor(store, bus);
        await processor.PlaceOrderAsync("SKU-1", 2);
        Assert.Equal(1, await processor.FlushAsync());
        Assert.Equal(("OrderPlaced", "SKU-1:2"), bus.Published.Single());
        Assert.Empty(await store.PendingAsync());
    }

    [Fact]
    public async Task BusFailure_KeepsPending_ForRetry()
    {
        var store = new MemoryOutboxStore();
        var bus = new RecordingBus { Fail = true };
        var processor = new OutboxProcessor(store, bus);
        await processor.PlaceOrderAsync("SKU-1", 1);
        await Assert.ThrowsAsync<InvalidOperationException>(() => processor.FlushAsync());
        Assert.Single(await store.PendingAsync());
        bus.Fail = false;
        Assert.Equal(1, await processor.FlushAsync());
    }
}
