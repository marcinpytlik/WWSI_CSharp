using System.Diagnostics;
var source = Enumerable.Range(1, 2_000_000).ToArray();
long Loop() { long sum = 0; foreach (int x in source) if ((x & 1) == 0) sum += x; return sum; }
long Linq() => source.Where(x => (x & 1) == 0).Sum(x => (long)x);
_ = Loop(); _ = Linq(); // warm-up
for (int attempt = 1; attempt <= 5; attempt++)
{
    var watch = Stopwatch.StartNew(); long a = Loop(); watch.Stop(); var loop = watch.Elapsed.TotalMilliseconds;
    watch.Restart(); long b = Linq(); watch.Stop();
    Console.WriteLine($"#{attempt}: loop={loop:F2} ms, LINQ={watch.Elapsed.TotalMilliseconds:F2} ms, same={a == b}");
}
Console.WriteLine("Uwaga: wynik pokazowy, nie mikrobenchmark.");
