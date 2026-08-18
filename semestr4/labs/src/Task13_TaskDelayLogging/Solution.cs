namespace Task13_TaskDelayLogging;

public interface ILogSink
{
    void Write(string message);
}

public sealed class ListLogSink : ILogSink
{
    public List<string> Lines { get; } = new();
    public void Write(string message) => Lines.Add(message);
}

public static class DelayLogger
{
    public static async Task LogDelayAsync(TimeSpan delay, ILogSink sink, TimeProvider? timeProvider = null)
    {
        timeProvider ??= TimeProvider.System;

        var start = timeProvider.GetUtcNow();
        sink.Write($"Start:{start:O}");

        await Task.Delay(delay);

        var end = timeProvider.GetUtcNow();
        sink.Write($"End:{end:O}");
        sink.Write($"ElapsedMs:{(end - start).TotalMilliseconds:0}");
    }
}
