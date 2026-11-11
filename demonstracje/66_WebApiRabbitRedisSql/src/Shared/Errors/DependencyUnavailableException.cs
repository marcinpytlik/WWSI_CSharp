namespace QueueCacheDemo.Errors;

public sealed class DependencyUnavailableException(string dependency, Exception? inner = null)
    : ApplicationExceptionBase("DEPENDENCY_UNAVAILABLE", $"{dependency} is temporarily unavailable.")
{
    public Exception? Cause { get; } = inner;
}
