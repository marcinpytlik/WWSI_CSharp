using Demo27;
using Xunit;

namespace Demo27.Tests;

public class PricePipelineTests
{
    [Fact]
    public void Discount_Then_Vat_Then_Shipping()
    {
        IPriceStep pipeline = new ShippingDecorator(
            new VatDecorator(new DiscountDecorator(new BasePrice(), 0.10m)), 12m);
        // 100 * 0.9 = 90; * 1.23 = 110.70; + 12 = 122.70
        Assert.Equal(122.70m, pipeline.Apply(100m));
    }

    [Fact]
    public void OrderMatters_ShippingVsDiscount()
    {
        var discountOnTopOfShipping = new DiscountDecorator(new ShippingDecorator(new BasePrice(), 20m), 0.10m);
        var shippingAfterDiscount = new ShippingDecorator(new DiscountDecorator(new BasePrice(), 0.10m), 20m);
        Assert.Equal(108.00m, discountOnTopOfShipping.Apply(100m));
        Assert.Equal(110.00m, shippingAfterDiscount.Apply(100m));
    }
}
