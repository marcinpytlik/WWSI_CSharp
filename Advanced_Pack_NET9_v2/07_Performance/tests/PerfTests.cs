
using Advanced.Performance;
using Xunit;

public class PerfTests
{
    [Fact]
    public void ForSum_Equals_LinqSum()
    {
        var b = new SumBench();
        Assert.Equal(b.LinqSum(), b.ForSum());
    }
}
