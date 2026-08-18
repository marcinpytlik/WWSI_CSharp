using System.Net;
using System.Net.Http.Json;
using Demo42;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demo42.Tests;

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

public class SeqLoggingTests : IClassFixture<IsolatedApiFactory>
{
    private readonly HttpClient _client;

    public SeqLoggingTests(IsolatedApiFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Post_EchoesCorrelationId_WithoutSeq()
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/v1/events")
        {
            Content = JsonContent.Create(new { message = "Hello Seq" })
        };
        req.Headers.Add("X-Correlation-Id", "cid-demo-42");
        var res = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        Assert.Equal("cid-demo-42", res.Headers.GetValues("X-Correlation-Id").Single());

        var row = await res.Content.ReadFromJsonAsync<EventRow>();
        Assert.Equal("cid-demo-42", row!.CorrelationId);
    }

    [Fact]
    public async Task Health_Ok()
    {
        var res = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}
