using Xunit;

namespace Task02_Average.Tests;

public sealed class AverageCalculatorTests
{
    [Fact]
    public void Average_ComputesCorrectly()
        => Assert.Equal(20d, Task02_Average.AverageCalculator.Average(new[] { 10,20,30 }));

    [Fact]
    public void Average_Empty_Throws()
        => Assert.Throws<ArgumentException>(() => Task02_Average.AverageCalculator.Average(Array.Empty<int>()));
}
