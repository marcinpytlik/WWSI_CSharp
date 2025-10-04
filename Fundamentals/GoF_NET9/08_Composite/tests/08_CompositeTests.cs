using GoF.Composite;
using Xunit;

public class CompositeTests
{
    [Fact]
    public void Counts_All_Nodes()
    {
        var g = new Group();
        g.Add(new Leaf()); g.Add(new Leaf());
        Assert.Equal(2, g.Count());
    }
}
