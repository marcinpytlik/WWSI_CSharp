using Demo65;
using Xunit;

namespace Demo65.Tests;

public class NullObjectTests
{
    [Fact]
    public void NoDiscount_LeavesPrice_WithoutNullCheck()
        => Assert.Equal(100, new Checkout(NoDiscount.Instance).Total(100));

    [Fact]
    public void PercentDiscount_Applies()
        => Assert.Equal(90, new Checkout(new PercentDiscount(10)).Total(100));
}
