using GoF.NullObject;
using Xunit;

public class NullObjectTests
{
    [Fact]
    public void Does_Not_Throw_When_Logging_NullObject()
    {
        var w = new Worker(new NullLogger());
        w.Do();
        Assert.True(true);
    }
}
