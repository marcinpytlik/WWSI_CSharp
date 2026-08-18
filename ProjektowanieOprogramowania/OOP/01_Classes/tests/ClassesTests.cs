using Oop.Classes;
using Xunit;

public class ClassesTests
{
    [Fact]
    public void Counter_Adds_And_Increments()
    {
        var c = new Counter();
        c.Inc();
        c.Add(2);
        Assert.Equal(3, c.Value);
    }
}
