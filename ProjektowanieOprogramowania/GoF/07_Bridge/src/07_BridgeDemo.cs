namespace GoF.Bridge;

public interface IRenderer { string DrawCircle(float x,float y,float r); }
public class VectorRenderer : IRenderer { public string DrawCircle(float x,float y,float r)=>$"vec:{x},{y},{r}"; }

public abstract class Shape { protected readonly IRenderer R; protected Shape(IRenderer r)=>R=r; public abstract string Draw(); }
public class Circle : Shape
{
    private readonly float _x, _y, _radius;
    public Circle(IRenderer renderer, float x, float y, float radius) : base(renderer)
    {
        _x = x;
        _y = y;
        _radius = radius;
    }
    public override string Draw() => R.DrawCircle(_x, _y, _radius);
}
