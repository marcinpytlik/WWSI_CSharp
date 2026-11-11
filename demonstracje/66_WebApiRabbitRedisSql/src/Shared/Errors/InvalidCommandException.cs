namespace QueueCacheDemo.Errors;

public sealed class InvalidCommandException(string reason)
    : ApplicationExceptionBase("INVALID_COMMAND", reason)
{
}
