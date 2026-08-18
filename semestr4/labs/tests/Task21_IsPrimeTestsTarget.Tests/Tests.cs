using Xunit;

namespace Task21_IsPrimeTestsTarget.Tests;

public sealed class PrimeTests
{
    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(4, false)]
    [InlineData(97, true)]
    public void IsPrime_Works(int n, bool expected)
        => Assert.Equal(expected, Task21_IsPrimeTestsTarget.Prime.IsPrime(n));
}
