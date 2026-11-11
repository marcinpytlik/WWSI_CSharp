namespace QueueCacheDemo.Errors;

public sealed class RequestConflictException(string message)
    : ApplicationExceptionBase("CONFLICT", message)
{
}
