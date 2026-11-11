namespace QueueCacheDemo.Errors;

// SafeCode is a stable public/application identifier; avoid embedding database details.
public abstract class ApplicationExceptionBase(string safeCode, string message) : Exception(message)
{
    public string SafeCode { get; } = safeCode;
}
