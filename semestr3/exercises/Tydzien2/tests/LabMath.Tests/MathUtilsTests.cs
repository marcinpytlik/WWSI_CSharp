using System;
using Xunit;
using LabMath;

public class MathUtilsTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(5, 120)]
    [InlineData(10, 3628800)]
    public void Factorial_Works(int n, long expected)
        => Assert.Equal(expected, MathUtils.Factorial(n));

    [Fact]
    public void Factorial_Throws_On_Negative()
        => Assert.Throws<ArgumentException>(() => MathUtils.Factorial(-1));

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(4, false)]
    [InlineData(13, true)]
    [InlineData(15, false)]
    [InlineData(97, true)]
    public void IsPrime_Works(int n, bool expected)
        => Assert.Equal(expected, MathUtils.IsPrime(n));

    [Fact]
    public void AverageOfTen_Works()
    {
        var arr = new[] { 2,4,6,8,10,12,14,16,18,20 };
        var avg = MathUtils.AverageOfTen(arr);
        Assert.Equal(11.0, avg, precision: 5);
    }

    [Fact]
    public void AverageOfTen_Throws_On_Wrong_Length()
        => Assert.Throws<ArgumentException>(() => MathUtils.AverageOfTen(new[] {1,2,3}));

    [Fact]
    public void AverageOfTen_Throws_On_Null()
        => Assert.Throws<ArgumentNullException>(() => MathUtils.AverageOfTen(null!));
}
