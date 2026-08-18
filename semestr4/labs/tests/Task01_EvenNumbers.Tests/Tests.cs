using Xunit;

namespace Task01_EvenNumbers.Tests;

public sealed class EvenNumbersTests
{
    [Fact]
    public void GetEven_ReturnsOnlyEven()
    {
        var result = Task01_EvenNumbers.EvenNumbers.GetEven(new[] { 1,2,3,4,5,6 });
        Assert.Equal(new[] { 2,4,6 }, result);
    }

    [Fact]
    public void GetEven_Empty_ReturnsEmpty()
        => Assert.Empty(Task01_EvenNumbers.EvenNumbers.GetEven(Array.Empty<int>()));
}
