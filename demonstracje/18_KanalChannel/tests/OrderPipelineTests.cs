using Demo18;
using Xunit;

namespace Demo18.Tests;

public class OrderPipelineTests
{
    [Fact]
    public async Task Produce_Then_Consume_PreservesOrder()
    {
        var pipeline = new OrderPipeline();
        var produce = pipeline.ProduceAsync([new("A", 1), new("B", 2)]);
        var items = await pipeline.ConsumeAsync();
        await produce;
        Assert.Equal(["A", "B"], items.Select(x => x.Sku));
    }

    [Fact]
    public async Task Empty_Completes()
    {
        var pipeline = new OrderPipeline();
        var produce = pipeline.ProduceAsync([]);
        var items = await pipeline.ConsumeAsync();
        await produce;
        Assert.Empty(items);
    }
}
