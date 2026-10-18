using System.Diagnostics;

PrintState("Początek");
Console.WriteLine("Alokuję tymczasowe tablice...");

for (int i = 0; i < 25_000; i++)
{
    byte[] buffer = new byte[4_096];
    buffer[0] = (byte)i;
    GC.KeepAlive(buffer);
}

PrintState("Po alokacjach");
using (var resource = new DemoResource())
{
    Console.WriteLine("Wewnątrz using: zasób aktywny.");
}
Console.WriteLine("Po using: Dispose zostało wywołane.");
Console.WriteLine("Wciśnij Enter, aby zakończyć (można obserwować proces z zewnątrz).");
Console.ReadLine();

static void PrintState(string label)
{
    Console.WriteLine($"{label}: pamięć zarządzana = {GC.GetTotalMemory(false):N0} B, gen0 = {GC.CollectionCount(0)}, gen1 = {GC.CollectionCount(1)}, gen2 = {GC.CollectionCount(2)}, PID = {Environment.ProcessId}");
}

sealed class DemoResource : IDisposable
{
    private bool _disposed;
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Console.WriteLine("DemoResource.Dispose(): zwolnienie zasobu (symulowane).");
    }
}
