using Oop.Interfaces;
using Xunit;

public class InterfacesPolymorphismTests
{
    [Fact]
    public void Strategy_Works()
    {
        var c = new Checkout(new Percent10());
        Assert.Equal(90m, c.Total(100m));
    }
}
