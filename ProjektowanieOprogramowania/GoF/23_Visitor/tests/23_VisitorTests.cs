using GoF.Visitor;
using Xunit;

public class VisitorTests
{
    [Fact]
    public void SumVisitor_Adds_Leaves()
    {
        var v = new SumVisitor();
        new Number(2).Accept(v);
        new Number(3).Accept(v);
        Assert.Equal(5, v.Sum);
    }

    [Fact]
    public void SumVisitor_Walks_Tree()
    {
        INode tree = new AddNode(new Number(2), new AddNode(new Number(3), new Number(4)));
        var v = new SumVisitor();
        tree.Accept(v);
        Assert.Equal(9, v.Sum);
    }

    [Fact]
    public void PrintVisitor_Renders_Expression()
    {
        INode tree = new AddNode(new Number(1), new Number(2));
        var v = new PrintVisitor();
        tree.Accept(v);
        Assert.Equal("( 1 + 2 )", v.Text);
    }
}
