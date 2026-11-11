using System.Diagnostics;

namespace QueueCacheDemo.Api.Middleware;

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try { await next(context); }
        finally
        {
            logger.LogInformation("HTTP {Method} {Path} -> {Status} in {Ms:F1}ms TraceId={TraceId}",
                context.Request.Method, context.Request.Path.Value, context.Response.StatusCode,
                sw.Elapsed.TotalMilliseconds, Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier);
        }
    }
}
