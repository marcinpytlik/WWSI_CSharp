using Xunit;

namespace Task19_SumNumbersLargeFile.Tests;

public sealed class NumberSummerTests
{
    [Fact]
    public void SumNumbers_SumsCorrectly()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        File.WriteAllLines(path, Enumerable.Range(1, 1000).Select(i => i.ToString()));

        var sum = Task19_SumNumbersLargeFile.NumberSummer.SumNumbers(path);

        Assert.Equal(1000L * 1001 / 2, sum);
        File.Delete(path);
    }
}
