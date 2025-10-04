using GoF.Decorator;
using Xunit;

public class DecoratorTests
{
    [Fact]
    public async Task Caches_Result()
    {
        IData d = new DataWithCache(new Data());
        var a = await d.GetAsync("x");
        var b = await d.GetAsync("x");
        Assert.Same(a, b);
    }
}
