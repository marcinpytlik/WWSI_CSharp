using System.Diagnostics;
using System.Net.Http.Json;
using Demo59;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demo59.Tests;

public sealed class IsolatedApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        builder.UseSetting("Testing", "true");
        builder.ConfigureServices(services =>
        {
            foreach (var descriptor in services.Where(d =>
                         d.ServiceType == typeof(EventsDb) ||
                         d.ServiceType == typeof(DbContextOptions<EventsDb>)).ToList())
                services.Remove(descriptor);
            services.AddDbContext<EventsDb>(o => o.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Dispose();
        base.Dispose(disposing);
    }
}

public class OtelTests : IClassFixture<IsolatedApiFactory>
{
    private readonly IsolatedApiFactory _factory;
    private readonly HttpClient _client;

    public OtelTests(IsolatedApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_CreatesActivity_AndSqlRow()
    {
        var names = new List<string>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = s => s.Name == "Demo59",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = a => names.Add(a.OperationName)
        };
        ActivitySource.AddActivityListener(listener);

        var res = await _client.PostAsJsonAsync("/api/v1/events", new { name = "Sala OTEL" });
        res.EnsureSuccessStatusCode();
        Assert.Contains("create-event", names);

        var list = await _client.GetFromJsonAsync<List<TraceEvent>>("/api/v1/events");
        Assert.Equal("Sala OTEL", Assert.Single(list!).Name);
    }
}
