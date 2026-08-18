using Demo32;
using Demo32.Api;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var testing = builder.Configuration.GetValue("Testing", false);
var hangfireEnabled = builder.Configuration.GetValue("Hangfire:Enabled", true) && !testing;

if (!testing)
{
    var cs = builder.Configuration.GetConnectionString("App")
             ?? throw new InvalidOperationException("Missing ConnectionStrings:App");
    builder.Services.AddDbContext<ReportsDb>(o => o.UseSqlServer(cs));
    if (hangfireEnabled)
    {
        builder.Services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(cs, new SqlServerStorageOptions
            {
                PrepareSchemaIfNecessary = false,
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }));
        builder.Services.AddSingleton<IJobQueue, HangfireJobQueue>();
    }
}

builder.Services.AddScoped<IReportProcessor, ReportProcessor>();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReportsDb>();
    await SqlRetry.WaitAsync(() => db.Database.EnsureCreatedAsync(), logger);
}

app.MapGet("/health", () => Results.Ok(new { status = "ok", role = "api" }));

app.MapGet("/api/v1/reports", async (ReportsDb db) =>
    await db.Reports.AsNoTracking().OrderByDescending(r => r.Id).ToListAsync());

app.MapGet("/api/v1/reports/{id:int}", async (int id, ReportsDb db) =>
    await db.Reports.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id) is { } report
        ? Results.Ok(report)
        : Results.NotFound());

app.MapPost("/api/v1/reports", async (CreateReport dto, ReportsDb db, IJobQueue jobs) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Trim().Length < 3)
        return Results.BadRequest(new { error = "Title must have at least 3 characters." });

    var report = new Report { Title = dto.Title.Trim(), Status = "Queued" };
    db.Reports.Add(report);
    await db.SaveChangesAsync();
    var jobId = jobs.EnqueueReport(report.Id);
    return Results.Accepted($"/api/v1/reports/{report.Id}", new { report.Id, report.Title, report.Status, jobId });
});

app.Run();

public sealed record CreateReport(string Title);
public partial class Program;
