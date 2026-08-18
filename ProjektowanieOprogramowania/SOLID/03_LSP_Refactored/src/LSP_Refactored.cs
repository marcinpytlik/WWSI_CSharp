namespace Solid.LSP.Ref;

public interface IShape { int Area(); }

public class Rectangle : IShape
{
    public int Width { get; init; }
    public int Height { get; init; }
    public int Area() => Width * Height;
}

public class Square : IShape
{
    public int A { get; init; }
    public int Area() => A * A;
}
