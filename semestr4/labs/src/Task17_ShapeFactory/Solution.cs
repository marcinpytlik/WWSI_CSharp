namespace Task17_ShapeFactory;

public interface IShape { double Area(); }

public sealed class Circle(double radius) : IShape
{
    public double Area() => Math.PI * radius * radius;
}

public sealed class Rectangle(double width, double height) : IShape
{
    public double Area() => width * height;
}

public static class ShapeFactory
{
    public static IShape Create(string type, double a, double b = 0)
        => type.Trim().ToLowerInvariant() switch
        {
            "circle" => new Circle(a),
            "rectangle" => new Rectangle(a, b),
            _ => throw new ArgumentException("Unknown shape type.", nameof(type))
        };
}
