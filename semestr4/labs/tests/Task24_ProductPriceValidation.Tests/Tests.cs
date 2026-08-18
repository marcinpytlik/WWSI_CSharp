using Xunit;

namespace Task24_ProductPriceValidation.Tests;

public sealed class ProductTests
{
    [Fact]
    public void NegativePrice_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new Task24_ProductPriceValidation.Product(1,"X",-1m));

    [Fact]
    public void PositivePrice_Works()
    {
        var p = new Task24_ProductPriceValidation.Product(1,"X",1m);
        Assert.Equal(1m, p.Price);
    }
}
