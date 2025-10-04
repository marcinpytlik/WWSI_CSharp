
using Advanced.LinqCollections;
using Xunit;

public class LinqDemoTests
{
    [Fact]
    public void Filters_And_Projects()
    {
        var data = new (string,bool)[]{("a",true),("b",false),("c",true)};
        var res = LinqDemo.SelectActiveUpper(data).ToArray();
        Assert.Equal(new[]{"A","C"}, res);
    }

    [Fact]
    public void Sums_Span()
    {
        var arr = new[]{1,2,3};
        Assert.Equal(6, LinqDemo.SumSpan(arr));
    }
}
