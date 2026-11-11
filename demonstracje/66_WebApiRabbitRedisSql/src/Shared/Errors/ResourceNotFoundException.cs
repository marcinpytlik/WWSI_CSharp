namespace QueueCacheDemo.Errors;

public sealed class ResourceNotFoundException(string resource)
    : ApplicationExceptionBase("NOT_FOUND", $"{resource} was not found.")
{
}
