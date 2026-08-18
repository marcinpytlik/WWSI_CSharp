using GoF.Flyweight;
using Xunit;

public class FlyweightTests
{
    [Fact]
    public void Reuses_Same_Instance()
    {
        var f = new GlyphFactory();
        var a = f.Get('a');
        var b = f.Get('a');
        Assert.Same(a, b);
    }
}
