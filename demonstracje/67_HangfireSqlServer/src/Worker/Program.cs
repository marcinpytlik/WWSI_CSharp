using Demo67.Shared;
using Hangfire;
using Hangfire.SqlServer;

var builder = Host.CreateApplicationBuilder(args);
var cs = builder.Configuration.GetConnectionString("Sql") ?? throw new InvalidOperationException("SQL connection missing");
builder.Services.AddHangfire(c => c
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(cs, new SqlServerStorageOptions
    {
        PrepareSchemaIfNecessary = true,
        TryAutoDetectSchemaDependentOptions = false,
        QueuePollInterval = TimeSpan.FromSeconds(1),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        UseRecommendedIsolationLevel = true
    }));
builder.Services.AddTransient(sp => new DemoJobs(cs, sp.GetRequiredService<ILogger<DemoJobs>>()));
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 1;
    options.Queues = ["default"];
    options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
});
await builder.Build().RunAsync();
