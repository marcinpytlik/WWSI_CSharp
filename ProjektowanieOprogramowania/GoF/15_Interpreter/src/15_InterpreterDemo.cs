namespace GoF.Interpreter;

public interface IExpr { int Eval(); }
public record Num(int V) : IExpr { public int Eval()=>V; }
public record Add(IExpr L, IExpr R) : IExpr { public int Eval()=> L.Eval()+R.Eval(); }
