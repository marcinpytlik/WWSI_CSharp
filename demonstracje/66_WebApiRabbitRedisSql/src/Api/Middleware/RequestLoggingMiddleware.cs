using System.Diagnostics;

namespace QueueCacheDemo.Api.Middleware;

/// <summary>Logs HTTP method, path, status and duration only; never logs request bodies or credentials.</summary>
public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var watch = Stopwatch.StartNew();
        try
        {
            await next(context);
        }
        finally
        {
            watch.Stop();
            logger.LogInformation("HTTP {Method} {Path} returned {StatusCode} in {ElapsedMs:F1} ms (trace {TraceId})",
                context.Request.Method, context.Request.Path.Value,
                context.Response.StatusCode, watch.Elapsed.TotalMilliseconds,
                context.TraceIdentifier);
        }
    }
}
