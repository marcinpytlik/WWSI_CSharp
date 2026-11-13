using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QueueCacheDemo.Errors;

namespace QueueCacheDemo.Api.ErrorHandling;

// Single place for unexpected faults. Normal endpoint results (401/403/404) stay unchanged.
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && context.RequestAborted.IsCancellationRequested)
        {
            logger.LogInformation("Client disconnected, trace {TraceId}", context.TraceIdentifier);
            return false; // Do not try to write a response to an aborted HTTP connection.
        }

        if (context.Response.HasStarted) return false;

        var status = exception switch
        {
            RequestValidationException => StatusCodes.Status400BadRequest,
            ResourceNotFoundException => StatusCodes.Status404NotFound,
            RequestConflictException => StatusCodes.Status409Conflict,
            DependencyUnavailableException => StatusCodes.Status503ServiceUnavailable,
            TimeoutException => StatusCodes.Status503ServiceUnavailable,
            Microsoft.EntityFrameworkCore.Storage.RetryLimitExceededException retry
                when FailureClassifier.Classify(retry) == FailureKind.Transient
                    => StatusCodes.Status503ServiceUnavailable,
            Microsoft.EntityFrameworkCore.DbUpdateException db
                when FailureClassifier.Classify(db) == FailureKind.Transient
                    => StatusCodes.Status503ServiceUnavailable,
            Microsoft.Data.SqlClient.SqlException sql
                when FailureClassifier.Classify(sql) == FailureKind.Transient
                    => StatusCodes.Status503ServiceUnavailable,
            // SQL connection faults may arrive wrapped in InvalidOperationException
            // when EF Core opens a connection or starts its execution strategy.
            InvalidOperationException wrapped
                when FailureClassifier.IsTransientSqlDependency(wrapped)
                    => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        var safe = exception as ApplicationExceptionBase;
        var code = safe?.SafeCode ?? (status == 503 ? "DEPENDENCY_UNAVAILABLE" : "UNEXPECTED_ERROR");

        if (status >= 500)
            logger.LogError(exception, "HTTP {StatusCode} {ErrorCode} trace={TraceId}",
                status, code, traceId);
        else
            logger.LogWarning("HTTP {StatusCode} {ErrorCode} trace={TraceId}",
                status, code, traceId);

        context.Response.Clear();
        if (status == StatusCodes.Status503ServiceUnavailable)
            context.Response.Headers["Retry-After"] = "5";

        await Results.Problem(
            statusCode: status,
            title: ReasonPhrases(status),
            detail: safe?.Message ?? (status == 503
                ? "A required service is temporarily unavailable."
                : "An unexpected error occurred."),
            type: $"https://httpstatuses.io/{status}",
            extensions: new Dictionary<string, object?>
            {
                ["traceId"] = traceId,
                ["code"] = code
            }).ExecuteAsync(context);

        return true;
    }

    private static string ReasonPhrases(int status) => status switch
    {
        400 => "Bad Request",
        404 => "Not Found",
        409 => "Conflict",
        503 => "Service Unavailable",
        _ => "Internal Server Error"
    };
}
