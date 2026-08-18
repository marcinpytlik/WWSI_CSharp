namespace GoF.Interpreter;

public interface IExpr
{
    int Eval();
}

public sealed record Num(int V) : IExpr
{
    public int Eval() => V;
}

public sealed record Add(IExpr L, IExpr R) : IExpr
{
    public int Eval() => L.Eval() + R.Eval();
}

public sealed record Sub(IExpr L, IExpr R) : IExpr
{
    public int Eval() => L.Eval() - R.Eval();
}

public sealed record Mul(IExpr L, IExpr R) : IExpr
{
    public int Eval() => L.Eval() * R.Eval();
}
