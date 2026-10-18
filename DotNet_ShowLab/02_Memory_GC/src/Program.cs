using System.Diagnostics;
var mode = args.FirstOrDefault()?.ToLowerInvariant() ?? "transient";
var retained = new List<byte[]>();
void Snapshot(string stage) => Console.WriteLine($"{stage}: PID={Environment.ProcessId}, allocated={GC.GetTotalAllocatedBytes(false):N0} B, managed={GC.GetTotalMemory(false):N0} B, gen0={GC.CollectionCount(0)}, gen1={GC.CollectionCount(1)}, gen2={GC.CollectionCount(2)}");
Snapshot("Before");
for (int i = 0; i < 20000; i++)
{
    var bytes = new byte[mode == "loh" ? 100_000 : 4096];
    bytes[0] = (byte)i;
    if (mode == "retained" && i < 10000) retained.Add(bytes);
    if (mode == "loh" && i % 100 == 0) retained.Add(bytes); // keep only a small sample
}
Snapshot("After");
Console.WriteLine($"Mode={mode}; kept={retained.Count} arrays. Press Enter to exit; observe with dotnet-counters.");
Console.ReadLine();
GC.KeepAlive(retained);
