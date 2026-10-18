const int workers = 8, each = 500_000;
int unsafeCounter = 0, safeCounter = 0;
Parallel.For(0, workers, _ =>
{
    for (int i = 0; i < each; i++)
    {
        int oldValue = unsafeCounter;
        if ((i & 255) == 0) Thread.Yield();
        unsafeCounter = oldValue + 1;
        Interlocked.Increment(ref safeCounter);
    }
});
Console.WriteLine($"Expected={workers * each:N0}, racy={unsafeCounter:N0}, Interlocked={safeCounter:N0}");
Console.WriteLine("Jeśli przypadkiem wynik racy jest poprawny, powtórz demonstrację.");
