using Demo01;
using Xunit;

namespace Demo01.Tests;

public class CalculatorTests
{
    [Theory]
    [InlineData("add", 2, 3, 5)]
    [InlineData("sub", 5, 1, 4)]
    [InlineData("mul", 3, 4, 12)]
    [InlineData("div", 10, 4, 2.5)]
    public void Compute_Works(string op, double a, double b, double expected)
        => Assert.Equal(expected, Calculator.Compute(op, a, b));

    [Fact]
    public void Div_ByZero_Throws()
        => Assert.Throws<DivideByZeroException>(() => Calculator.Compute("div", 1, 0));

    [Fact]
    public void App_Add_Exit0()
    {
        var output = new StringWriter();
        var error = new StringWriter();
        var code = App.Run(["add", "2", "3"], output, error);
        Assert.Equal(0, code);
        Assert.Equal("5" + Environment.NewLine, output.ToString());
    }
}
