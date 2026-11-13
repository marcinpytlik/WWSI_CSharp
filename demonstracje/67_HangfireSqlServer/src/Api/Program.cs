using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Demo67.Shared;

var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("Sql") ?? throw new InvalidOperationException("SQL connection missing");
builder.Services.AddHangfire(c => c
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(cs, new SqlServerStorageOptions
    {
        PrepareSchemaIfNecessary = true, // LAB ONLY
        TryAutoDetectSchemaDependentOptions = false,
        QueuePollInterval = TimeSpan.FromSeconds(1),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        UseRecommendedIsolationLevel = true
    }));
builder.Services.AddTransient(sp => new DemoJobs(cs, sp.GetRequiredService<ILogger<DemoJobs>>()));
var app = builder.Build();
// UNAUTHENTICATED DASHBOARD: isolated localhost lab only; never expose publicly.
app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = [new LabDashboardAccess()] });
app.MapGet("/health/live", () => Results.Ok(new { status = "alive" }));

app.MapPost("/jobs/fire", (IBackgroundJobClient jobs, WorkInput input) =>
{
    if (input.WorkSeconds is < 0 or > 60) return Results.BadRequest();
    var key = Guid.NewGuid().ToString("N");
    var id = jobs.Enqueue<DemoJobs>(j => j.ExecuteAsync(key, "fire", input.WorkSeconds));
    return Results.Accepted($"/jobs/{id}", new { jobId = id, runKey = key });
});
app.MapPost("/jobs/delay", (IBackgroundJobClient jobs, DelayInput input) =>
{
    if (input.WorkSeconds is < 0 or > 60 || input.DelaySeconds is < 1 or > 3600)
        return Results.BadRequest();
    var key = Guid.NewGuid().ToString("N");
    var id = jobs.Schedule<DemoJobs>(j => j.ExecuteAsync(key, "delayed", input.WorkSeconds), TimeSpan.FromSeconds(input.DelaySeconds));
    return Results.Accepted($"/jobs/{id}", new { jobId = id, runKey = key });
});
app.MapPost("/jobs/continuation", (IBackgroundJobClient jobs) =>
{
    var key = Guid.NewGuid().ToString("N");
    var parent = jobs.Enqueue<DemoJobs>(j => j.ExecuteAsync(key + "-parent", "parent", 0));
    var child = jobs.ContinueJobWith<DemoJobs>(parent, j => j.ExecuteAsync(key + "-child", "continuation", 0));
    return Results.Accepted($"/jobs/{child}", new { parentJobId = parent, childJobId = child, runKey = key });
});
app.MapPost("/jobs/fail-once", (IBackgroundJobClient jobs) =>
{
    var key = Guid.NewGuid().ToString("N");
    var id = jobs.Enqueue<DemoJobs>(j => j.FailOnceAsync(key));
    return Results.Accepted($"/jobs/{id}", new { jobId = id, runKey = key });
});
app.MapPost("/jobs/recurring", (IRecurringJobManager jobs) =>
{
    jobs.AddOrUpdate<DemoJobs>("demo67-heartbeat", j => j.RecurringHeartbeatAsync(), Cron.Minutely());
    return Results.Ok(new { recurringJobId = "demo67-heartbeat", schedule = "every minute" });
});
app.MapDelete("/jobs/recurring", (IRecurringJobManager jobs) =>
{
    jobs.RemoveIfExists("demo67-heartbeat");
    return Results.NoContent();
});
app.MapGet("/jobs/{id}", (string id) =>
{
    var monitor = JobStorage.Current.GetMonitoringApi();
    var detail = monitor.JobDetails(id);
    if (detail is null) return Results.NotFound();
    return Results.Ok(new { jobId = id, latestState = detail.History.FirstOrDefault()?.StateName,
        history = detail.History.Select(x=>x.StateName).ToArray() });
});
app.Run();

public sealed record WorkInput(int WorkSeconds);
public sealed record DelayInput(int WorkSeconds, int DelaySeconds);
public sealed class LabDashboardAccess : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true; // localhost-bound demo only
}
