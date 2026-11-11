namespace QueueCacheDemo.Errors;

// SafeCode is a stable public/application identifier; avoid embedding database details.
public abstract class ApplicationExceptionBase(string safeCode, string message, Exception? inner = null)
    : Exception(message, inner)
{
    public string SafeCode { get; } = safeCode;
}
