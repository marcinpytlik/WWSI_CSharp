using Microsoft.Extensions.Diagnostics.HealthChecks;

var probe = new ProbeHealthCheck();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(probe);
builder.Services.AddHealthChecks().AddCheck("probe", () => probe.Healthy
    ? HealthCheckResult.Healthy("sql-or-cache ok")
    : HealthCheckResult.Unhealthy("dependency down"));
var app = builder.Build();

app.MapGet("/api/v1/ready", (ProbeHealthCheck probe) =>
{
    probe.Healthy = true;
    return Results.Ok(new { ready = true });
});
app.MapGet("/api/v1/break", (ProbeHealthCheck probe) =>
{
    probe.Healthy = false;
    return Results.Ok(new { ready = false });
});
app.MapHealthChecks("/health");
app.Run();

public sealed class ProbeHealthCheck : IHealthCheck
{
    public bool Healthy { get; set; } = true;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(Healthy
            ? HealthCheckResult.Healthy("sql-or-cache ok")
            : HealthCheckResult.Unhealthy("dependency down"));
}

public partial class Program;
