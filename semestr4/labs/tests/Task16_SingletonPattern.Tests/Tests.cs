using Xunit;

namespace Task16_SingletonPattern.Tests;

public sealed class SingletonLoggerTests
{
    [Fact]
    public void Singleton_IsSameInstance()
    {
        var a = Task16_SingletonPattern.SingletonLogger.Instance;
        var b = Task16_SingletonPattern.SingletonLogger.Instance;

        Assert.Same(a, b);
    }
}
