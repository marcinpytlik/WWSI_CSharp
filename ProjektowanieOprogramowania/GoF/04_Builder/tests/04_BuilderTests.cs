using GoF.Builder;
using Xunit;

public class BuilderTests
{
    [Fact]
    public void Builds_Query()
    {
        var q = new QueryBuilder().From("Products").Where("Price > 10").Build();
        Assert.Equal("SELECT * FROM Products WHERE Price > 10", q);
    }
}
