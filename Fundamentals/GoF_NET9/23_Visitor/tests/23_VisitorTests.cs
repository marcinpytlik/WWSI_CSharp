using GoF.Visitor;
using Xunit;

public class VisitorTests
{
    [Fact]
    public void Sums_Numbers()
    {
        var v = new SumVisitor();
        new Number(2).Accept(v);
        new Number(3).Accept(v);
        Assert.Equal(5, v.Sum);
    }
}
