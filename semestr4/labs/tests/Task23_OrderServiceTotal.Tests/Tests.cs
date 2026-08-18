using Xunit;

namespace Task23_OrderServiceTotal.Tests;

public sealed class OrderServiceTests
{
    [Fact]
    public void CalculateTotal_Sums()
    {
        var svc = new Task23_OrderServiceTotal.OrderService();
        var total = svc.CalculateTotal(new[]
        {
            new Task23_OrderServiceTotal.Product(1,"A",10m),
            new Task23_OrderServiceTotal.Product(2,"B",5.5m),
        });

        Assert.Equal(15.5m, total);
    }
}
