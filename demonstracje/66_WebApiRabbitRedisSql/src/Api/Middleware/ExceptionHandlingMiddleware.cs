namespace QueueCacheDemo.Api.Middleware;

/// <summary>Centralized 500 ProblemDetails; stack traces stay in server logs only.</summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogInformation("HTTP request cancelled (trace {TraceId})", context.TraceIdentifier);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled HTTP error (trace {TraceId})", context.TraceIdentifier);
            if (context.Response.HasStarted) throw;
            context.Response.Clear();
            await Results.Problem(
                title: "Internal Server Error",
                detail: "An unexpected error occurred. Contact support with the traceId.",
                statusCode: StatusCodes.Status500InternalServerError,
                extensions: new Dictionary<string, object?> { ["traceId"] = context.TraceIdentifier })
                .ExecuteAsync(context);
        }
    }
}
