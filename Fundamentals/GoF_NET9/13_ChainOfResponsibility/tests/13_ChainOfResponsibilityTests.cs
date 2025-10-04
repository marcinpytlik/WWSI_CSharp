using GoF.Chain;
using Xunit;

public class ChainTests
{
    [Fact]
    public void Passes_Through_Handlers()
    {
        var trim = new TrimHandler();
        trim.SetNext(new LowerHandler());
        Assert.Equal("abc", trim.Handle("  AbC  "));
    }
}
