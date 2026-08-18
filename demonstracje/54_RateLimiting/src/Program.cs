using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var permit = builder.Configuration.GetValue("RateLimit:Permit", 5);
var windowSeconds = builder.Configuration.GetValue("RateLimit:WindowSeconds", 10);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("api", limiter =>
    {
        limiter.PermitLimit = permit;
        limiter.Window = TimeSpan.FromSeconds(windowSeconds);
        limiter.QueueLimit = 0;
        limiter.AutoReplenishment = true;
    });
});

var app = builder.Build();
app.UseRateLimiter();
app.MapGet("/api/v1/ping", () => Results.Ok(new { status = "ok" }))
    .RequireRateLimiting("api");
app.Run();

public partial class Program;
