using Xunit;

namespace Task12_AsyncExceptionHandling.Tests;

public sealed class SafeTests
{
    [Fact]
    public async Task TryAsync_ReturnsOk()
    {
        var r = await Task12_AsyncExceptionHandling.Safe.TryAsync(async () => { await Task.Delay(1); return 123; });
        Assert.True(r.IsSuccess);
        Assert.Equal(123, r.Value);
    }

    [Fact]
    public async Task TryAsync_ReturnsFail()
    {
        var r = await Task12_AsyncExceptionHandling.Safe.TryAsync<int>(() => throw new InvalidOperationException("boom"));
        Assert.False(r.IsSuccess);
        Assert.Contains("boom", r.Error);
    }
}
