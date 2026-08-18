using Xunit;

namespace Task13_TaskDelayLogging.Tests;

public sealed class DelayLoggerTests
{
    [Fact]
    public async Task LogDelayAsync_WritesThreeLines()
    {
        var sink = new Task13_TaskDelayLogging.ListLogSink();
        await Task13_TaskDelayLogging.DelayLogger.LogDelayAsync(TimeSpan.FromMilliseconds(1), sink);

        Assert.Equal(3, sink.Lines.Count);
        Assert.StartsWith("Start:", sink.Lines[0]);
        Assert.StartsWith("End:", sink.Lines[1]);
        Assert.StartsWith("ElapsedMs:", sink.Lines[2]);
    }
}
