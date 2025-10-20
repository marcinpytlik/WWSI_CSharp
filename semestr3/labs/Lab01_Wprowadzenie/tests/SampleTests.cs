using System.IO;
using Xunit;
using Lab01;

public class SampleTests
{
    [Theory]
    [InlineData(2, 2, 4)]
    [InlineData(-1, 1, 0)]
    [InlineData(0, 0, 0)]
    public void Add_Works(int a, int b, int expected)
        => Assert.Equal(expected, Calculator.Add(a, b));

    [Theory]
    [InlineData("5", 5)]
    [InlineData("-10", -10)]
    public void ParseInt_Works(string input, int expected)
        => Assert.Equal(expected, Calculator.ParseInt(input));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ParseInt_Throws_On_NullOrEmpty(string? input)
        => Assert.ThrowsAny<System.Exception>(() => Calculator.ParseInt(input));

    [Fact]
    public void App_Run_Add_FromArgs()
    {
        var args = new[] {"add", "3", "7"};
        var input = new StringReader("");
        var output = new StringWriter();
        var error = new StringWriter();
        var code = App.Run(args, input, output, error);
        Assert.Equal(0, code);
        Assert.Equal("10" + System.Environment.NewLine, output.ToString());
    }
}
