using Xunit;

namespace Task17_ShapeFactory.Tests;

public sealed class ShapeFactoryTests
{
    [Fact]
    public void Create_Circle_Area()
    {
        var s = Task17_ShapeFactory.ShapeFactory.Create("circle", 2);
        Assert.True(s.Area() > 12.56 && s.Area() < 12.57);
    }

    [Fact]
    public void Create_Rectangle_Area()
    {
        var s = Task17_ShapeFactory.ShapeFactory.Create("rectangle", 3, 4);
        Assert.Equal(12, s.Area(), 6);
    }
}
