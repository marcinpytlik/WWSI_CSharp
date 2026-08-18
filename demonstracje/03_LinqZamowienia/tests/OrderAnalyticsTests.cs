using Demo03;
using Xunit;

namespace Demo03.Tests;

public class OrderAnalyticsTests
{
    private static readonly Order[] Data =
    [
        new("Ada", "Book", 2, 40),
        new("Ada", "Pen", 10, 2),
        new("Jan", "Book", 1, 40)
    ];

    [Fact]
    public void TotalFor_Ada_Is100()
        => Assert.Equal(100m, OrderAnalytics.TotalFor(Data, "Ada"));

    [Fact]
    public void TopProducts_PenFirstByQty()
    {
        var top = OrderAnalytics.TopProducts(Data, 1);
        Assert.Equal("Pen", top[0].Product);
        Assert.Equal(10, top[0].Qty);
    }

    [Fact]
    public void Expensive_FiltersByLineTotal()
        => Assert.Single(OrderAnalytics.Expensive(Data, 80));
}
