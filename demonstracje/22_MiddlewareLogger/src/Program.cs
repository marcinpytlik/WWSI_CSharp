var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (ctx, next) =>
{
    var cid = ctx.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString("N");
    ctx.Response.Headers["X-Correlation-Id"] = cid;
    ctx.Items["cid"] = cid;
    using var scope = app.Logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = cid });
    await next();
});

app.Use(async (ctx, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var cid = ctx.Items["cid"] as string ?? "";
        app.Logger.LogError(ex, "Unhandled {CorrelationId}", cid);
        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
        ctx.Response.ContentType = "application/problem+json";
        await ctx.Response.WriteAsJsonAsync(new
        {
            type = "https://httpstatuses.com/500",
            title = "Server error",
            status = 500,
            correlationId = cid
        });
    }
});

app.MapGet("/api/v1/ok", () => Results.Ok(new { status = "ok" }));
app.MapGet("/api/v1/boom", (HttpContext _) => throw new InvalidOperationException("demo-fail"));

app.Run();

public partial class Program;
