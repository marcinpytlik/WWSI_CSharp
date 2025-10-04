using Oop.Composition;
using Xunit;

public class CompositionVsInheritanceTests
{
    [Fact]
    public void Prefers_Composition()
    {
        var r = new Report(new Percent10());
        Assert.Equal(90m, r.Price(100m));
    }
}
