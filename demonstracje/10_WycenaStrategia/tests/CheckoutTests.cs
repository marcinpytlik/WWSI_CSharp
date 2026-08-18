using Demo10;
using Xunit;

namespace Demo10.Tests;

public class CheckoutTests
{
    [Fact]
    public void Standard_Under100_PaysShipping()
        => Assert.Equal(113.16m, Checkout.Gross(80m, "standard")); // (80+12)*1.23

    [Fact]
    public void Standard_Over100_FreeShipping()
        => Assert.Equal(147.60m, Checkout.Gross(120m, "standard")); // 120*1.23

    [Fact]
    public void Express_Flat25()
        => Assert.Equal(129.15m, Checkout.Gross(80m, "express")); // (80+25)*1.23

    [Fact]
    public void Unknown_Throws()
        => Assert.Throws<ArgumentException>(() => ShippingFactory.Create("drone"));
}
