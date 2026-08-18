using System.Threading.Channels;

namespace Demo18;

public sealed record OrderMessage(string Sku, int Qty);

public sealed class OrderPipeline
{
    private readonly Channel<OrderMessage> _channel = Channel.CreateBounded<OrderMessage>(8);

    public ChannelWriter<OrderMessage> Writer => _channel.Writer;
    public ChannelReader<OrderMessage> Reader => _channel.Reader;

    public async Task ProduceAsync(IEnumerable<OrderMessage> messages, CancellationToken cancellationToken = default)
    {
        foreach (var message in messages)
            await Writer.WriteAsync(message, cancellationToken);
        Writer.Complete();
    }

    public async Task<IReadOnlyList<OrderMessage>> ConsumeAsync(CancellationToken cancellationToken = default)
    {
        var result = new List<OrderMessage>();
        await foreach (var message in Reader.ReadAllAsync(cancellationToken))
            result.Add(message);
        return result;
    }
}

public static class Program
{
    public static async Task<int> Main()
    {
        var pipeline = new OrderPipeline();
        var produce = pipeline.ProduceAsync([new("SKU-1", 2), new("SKU-2", 1)]);
        var consumed = await pipeline.ConsumeAsync();
        await produce;
        Console.WriteLine($"consumed={consumed.Count}");
        return 0;
    }
}
