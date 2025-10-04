using Xunit;
using LabApp;

public class CalculatorTests
{
    [Theory]
    [InlineData(2, 2, 4)]
    [InlineData(-1, 1, 0)]
    [InlineData(0, 0, 0)]
    public void Add_Works(int a, int b, int expected)
        => Assert.Equal(expected, Calculator.Add(a,b));

    [Theory]
    [InlineData(2, 2, 0)]
    [InlineData(10, 3, 7)]
    public void Sub_Works(int a, int b, int expected)
        => Assert.Equal(expected, Calculator.Sub(a,b));
}
