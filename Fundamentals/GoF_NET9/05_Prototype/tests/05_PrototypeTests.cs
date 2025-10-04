using GoF.Prototype;
using Xunit;

public class PrototypeTests
{
    [Fact]
    public void Clone_Creates_Copy()
    {
        var a = new EmailTemplate("Hi","Body");
        var b = a.Clone();
        Assert.Equal(a, b);
        Assert.NotSame(a, b);
    }
}
