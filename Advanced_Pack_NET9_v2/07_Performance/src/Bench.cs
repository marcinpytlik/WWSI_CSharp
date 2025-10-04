
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Advanced.Performance;

public class SumBench
{
    private int[] data = Enumerable.Range(0, 10_000).ToArray();

    [Benchmark]
    public int LinqSum() => data.Sum();

    [Benchmark]
    public int ForSum()
    {
        int s = 0;
        for (int i=0;i<data.Length;i++) s += data[i];
        return s;
    }
}

public class Program
{
    public static void Main(string[] args) => BenchmarkRunner.Run<SumBench>();
}
