using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using Demo32;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demo32.Tests;

public sealed class RecordingJobQueue : IJobQueue
{
    public ConcurrentBag<int> Enqueued { get; } = [];
    public string EnqueueReport(int reportId)
    {
        Enqueued.Add(reportId);
        return $"job-{reportId}";
    }
}

public sealed class IsolatedApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    public RecordingJobQueue Jobs { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        builder.UseSetting("Testing", "true");
        builder.UseSetting("Hangfire:Enabled", "false");
        builder.ConfigureServices(services =>
        {
            foreach (var descriptor in services.Where(d =>
                         d.ServiceType == typeof(ReportsDb) ||
                         d.ServiceType == typeof(DbContextOptions<ReportsDb>)).ToList())
                services.Remove(descriptor);
            services.AddDbContext<ReportsDb>(o => o.UseSqlite(_connection));
            services.AddSingleton<IJobQueue>(Jobs);
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Dispose();
        base.Dispose(disposing);
    }
}

public class ApiTests : IClassFixture<IsolatedApiFactory>
{
    private readonly IsolatedApiFactory _factory;
    private readonly HttpClient _client;

    public ApiTests(IsolatedApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_EnqueuesJob_AndListsReport()
    {
        var create = await _client.PostAsJsonAsync("/api/v1/reports", new { title = "Sales Q1" });
        Assert.Equal(HttpStatusCode.Accepted, create.StatusCode);
        Assert.Contains(1, _factory.Jobs.Enqueued);

        var list = await _client.GetFromJsonAsync<List<Report>>("/api/v1/reports");
        var saved = Assert.Single(list!);
        Assert.Equal("Sales Q1", saved.Title);
        Assert.Equal("Queued", saved.Status);
    }

    [Fact]
    public async Task ShortTitle_400()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/reports", new { title = "ab" });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }
}
