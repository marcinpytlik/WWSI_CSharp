namespace Demo13;

public abstract class Shape
{
    public abstract double Area();
}

public sealed class Circle : Shape
{
    public Circle(double radius)
    {
        if (radius <= 0) throw new ArgumentOutOfRangeException(nameof(radius));
        Radius = radius;
    }

    public double Radius { get; }
    public override double Area() => Math.PI * Radius * Radius;
}

public sealed class Rectangle : Shape
{
    public Rectangle(double width, double height)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        Width = width;
        Height = height;
    }

    public double Width { get; }
    public double Height { get; }
    public override double Area() => Width * Height;
}

public static class ShapeCatalog
{
    public static double TotalArea(IEnumerable<Shape> shapes) => shapes.Sum(s => s.Area());
}

public static class Program
{
    public static int Main()
    {
        Shape[] shapes = [new Circle(2), new Rectangle(3, 4)];
        Console.WriteLine($"total={ShapeCatalog.TotalArea(shapes):0.00}");
        return 0;
    }
}
