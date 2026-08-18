using Demo26;
using Xunit;

namespace Demo26.Tests;

public class OrderStateTests
{
    [Fact]
    public void HappyPath_New_Paid_Shipped()
    {
        var order = new Order("SKU-1", 1);
        order.Pay();
        order.Ship();
        Assert.Equal(OrderState.Shipped, order.State);
    }

    [Fact]
    public void Ship_BeforePay_Throws()
    {
        var order = new Order("SKU-1", 1);
        Assert.Throws<InvalidOperationException>(() => order.Ship());
    }

    [Fact]
    public void CannotCancel_Shipped()
    {
        var order = new Order("SKU-1", 1);
        order.Pay();
        order.Ship();
        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }
}
