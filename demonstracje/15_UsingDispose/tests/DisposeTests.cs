using Demo15;
using Xunit;

namespace Demo15.Tests;

public class DisposeTests
{
    [Fact]
    public void Using_DisposesEvenOnException()
    {
        FakeConnection? captured = null;
        try
        {
            using var conn = new FakeConnection();
            captured = conn;
            conn.Execute("OK");
            throw new InvalidOperationException("boom");
        }
        catch (InvalidOperationException)
        {
            Assert.True(captured!.Disposed);
        }
    }

    [Fact]
    public void AfterDispose_ExecuteThrows()
    {
        var conn = new FakeConnection();
        conn.Dispose();
        Assert.Throws<ObjectDisposedException>(() => conn.Execute("SELECT 1"));
    }

    [Fact]
    public void Leak_RemainsOpen()
    {
        var leaked = ConnectionRunner.RunAndLeak("SELECT 1");
        Assert.False(leaked.Disposed);
        Assert.True(leaked.IsOpen);
        leaked.Dispose();
    }
}
