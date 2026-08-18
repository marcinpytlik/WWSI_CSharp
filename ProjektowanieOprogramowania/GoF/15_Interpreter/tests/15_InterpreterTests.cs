using GoF.Interpreter;
using Xunit;

public class InterpreterTests
{
    [Fact]
    public void Evaluates_Simple_Addition()
    {
        IExpr expr = new Add(new Num(2), new Num(3));
        Assert.Equal(5, expr.Eval());
    }

    [Fact]
    public void Evaluates_Nested_Expression()
    {
        // (2 + 3) * (10 - 4) = 30
        IExpr expr = new Mul(new Add(new Num(2), new Num(3)), new Sub(new Num(10), new Num(4)));
        Assert.Equal(30, expr.Eval());
    }

    [Fact]
    public void Subtracts_And_Multiplies()
    {
        Assert.Equal(-1, new Sub(new Num(2), new Num(3)).Eval());
        Assert.Equal(12, new Mul(new Num(3), new Num(4)).Eval());
    }
}
