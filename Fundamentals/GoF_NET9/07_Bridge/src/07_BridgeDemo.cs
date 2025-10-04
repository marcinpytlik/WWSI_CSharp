namespace GoF.Bridge;

public interface IRenderer { string DrawCircle(float x,float y,float r); }
public class VectorRenderer : IRenderer { public string DrawCircle(float x,float y,float r)=>$"vec:{x},{y},{r}"; }

public abstract class Shape { protected readonly IRenderer R; protected Shape(IRenderer r)=>R=r; public abstract string Draw(); }
public class Circle : Shape { float x,y,r; public Circle(IRenderer r,float x,float y,float r):base(r){this.x=x;this.y=y;this.r=r;} public override string Draw()=>R.DrawCircle(x,y,r); }
