using Xunit;

namespace Task05_SortStringsDesc.Tests;

public sealed class SorterTests
{
    [Fact]
    public void SortDescending_SortsCorrectly()
    {
        var result = Task05_SortStringsDesc.Sorter.SortDescending(new[] { "b", "a", "c" });
        Assert.Equal(new[] { "c", "b", "a" }, result);
    }
}
