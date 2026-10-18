using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<RequestStamp>();
var app = builder.Build();

app.Use(async (context, next) =>
{
    var correlationId = Guid.NewGuid().ToString("N");
    context.Response.Headers["X-Correlation-ID"] = correlationId;
    context.Items["CorrelationId"] = correlationId;
    var stamp = context.RequestServices.GetRequiredService<RequestStamp>();
    var watch = Stopwatch.StartNew();

    app.Logger.LogInformation("START {Method} {Path}; correlation={Correlation}; scope={Scope}",
        context.Request.Method, context.Request.Path, correlationId, stamp.Id);
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Błąd żądania correlation={Correlation}", correlationId);
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "Internal Server Error", correlationId });
        }
    }
    finally
    {
        watch.Stop();
        app.Logger.LogInformation("END {Status}; correlation={Correlation}; scope={Scope}; elapsedMs={Elapsed}",
            context.Response.StatusCode, correlationId, stamp.Id, watch.ElapsedMilliseconds);
    }
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.MapGet("/orders/{id:int}", (int id, RequestStamp scope, HttpContext ctx) =>
{
    app.Logger.LogInformation("Endpoint order={Order}; correlation={Correlation}; scope={Scope}",
        id, ctx.Items["CorrelationId"], scope.Id);
    return Results.Ok(new { id, status = "Created", scopeId = scope.Id, correlationId = ctx.Items["CorrelationId"] });
});
app.MapGet("/fail", () => { throw new InvalidOperationException("Symulowany błąd dla pokazu"); });
app.Run();

sealed class RequestStamp
{
    public Guid Id { get; } = Guid.NewGuid();
}
