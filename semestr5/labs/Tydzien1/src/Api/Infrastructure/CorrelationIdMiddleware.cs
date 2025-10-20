namespace Api.Infrastructure.Middleware
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next;
        public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(HeaderName, out var correlationId) || string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString("N");
            }
            context.Response.Headers[HeaderName] = correlationId!;
            var started = DateTime.UtcNow;
            try
            {
                await _next(context);
            }
            finally
            {
                var elapsed = DateTime.UtcNow - started;
                context.Response.Headers["X-Elapsed-ms"] = elapsed.TotalMilliseconds.ToString("F0");
            }
        }
    }
}
