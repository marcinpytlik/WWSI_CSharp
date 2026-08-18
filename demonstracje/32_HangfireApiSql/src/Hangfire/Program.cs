using Demo32;
using Demo32.HangfireHost;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("App")
         ?? throw new InvalidOperationException("Missing ConnectionStrings:App");

builder.Services.AddDbContext<ReportsDb>(o => o.UseSqlServer(cs));
builder.Services.AddScoped<IReportProcessor, ReportProcessor>();
builder.Services.AddHangfire(cfg => cfg
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(cs, new SqlServerStorageOptions
    {
        PrepareSchemaIfNecessary = true,
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));
builder.Services.AddHangfireServer(options => options.WorkerCount = 1);

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReportsDb>();
    await SqlRetry.WaitAsync(() => db.Database.EnsureCreatedAsync(), logger);
}

app.MapGet("/health", () => Results.Ok(new { status = "ok", role = "hangfire" }));
app.MapHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new AllowAllDashboardFilter()]
});
app.Run();

