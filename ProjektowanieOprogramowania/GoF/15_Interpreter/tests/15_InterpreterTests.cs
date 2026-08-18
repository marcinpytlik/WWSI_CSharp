using GoF.Interpreter;
using Xunit;

public class InterpreterTests
{
    [Fact]
    public void Evaluates_Simple_Expression()
    {
        IExpr expr = new Add(new Num(2), new Num(3));
        Assert.Equal(5, expr.Eval());
    }
}
