using GoF.Singleton;
using Xunit;

public class SingletonTests
{
    [Fact]
    public void Instance_Is_Single()
    {
        var a = AppCache.Instance;
        var b = AppCache.Instance;
        a.Inc();
        Assert.Same(a, b);
        Assert.Equal(1, b.Counter);
    }
}
