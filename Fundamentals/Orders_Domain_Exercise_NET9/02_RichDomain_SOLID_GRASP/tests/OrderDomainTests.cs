
using Xunit;
using Orders.Rich.Domain;
using Orders.Rich.Domain.Primitives;

public class OrderDomainTests
{
    [Fact]
    public void TotalNet_Sums_Lines()
    {
        var o = Order.Create(Email.Create("a@b"), DateTime.UtcNow);
        o.AddLine("SKU", 2, new Money(50m, "PLN"));
        Assert.Equal(new Money(100m, "PLN"), o.TotalNet());
    }
}
