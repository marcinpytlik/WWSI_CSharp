using GoF.Strategy;
using Xunit;

public class StrategyTests
{
    [Fact]
    public void Applies_Discount()
    {
        var c = new Checkout(new Percentage());
        Assert.Equal(90m, c.Pay(100m));
    }
}
