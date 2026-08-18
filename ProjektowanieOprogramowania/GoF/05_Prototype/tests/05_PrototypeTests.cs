using GoF.Prototype;
using Xunit;

public class PrototypeTests
{
    [Fact]
    public void Duplicate_Creates_Copy()
    {
        var a = new EmailTemplate("Hi","Body");
        var b = a.Duplicate();
        Assert.Equal(a, b);
        Assert.NotSame(a, b);
    }
}
