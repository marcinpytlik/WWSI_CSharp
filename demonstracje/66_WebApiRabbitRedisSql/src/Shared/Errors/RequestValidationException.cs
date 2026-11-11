namespace QueueCacheDemo.Errors;

public sealed class RequestValidationException(string message)
    : ApplicationExceptionBase("VALIDATION_ERROR", message)
{
}
