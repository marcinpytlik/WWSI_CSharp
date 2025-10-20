namespace Api.Infrastructure;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string Header = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx)
    {
        if (!ctx.Request.Headers.TryGetValue(Header, out var id) || string.IsNullOrWhiteSpace(id))
            id = Guid.NewGuid().ToString("N");
        ctx.Response.Headers[Header] = id!;
        await _next(ctx);
    }
}
