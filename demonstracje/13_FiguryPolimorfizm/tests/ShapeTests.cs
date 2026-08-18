using Demo13;
using Xunit;

namespace Demo13.Tests;

public class ShapeTests
{
    [Fact]
    public void Circle_Area()
    {
        Assert.Equal(Math.PI * 4, new Circle(2).Area(), 10);
    }

    [Fact]
    public void TotalArea_UsesPolymorphism()
    {
        Shape[] shapes = [new Circle(1), new Rectangle(2, 3)];
        Assert.Equal(Math.PI + 6, ShapeCatalog.TotalArea(shapes), 10);
    }

    [Fact]
    public void InvalidRadius_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new Circle(0));
}
