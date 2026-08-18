using System.Net;
using System.Net.Http.Json;
using Demo41;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demo41.Tests;

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
                         d.ServiceType == typeof(NotesDb) ||
                         d.ServiceType == typeof(DbContextOptions<NotesDb>)).ToList())
                services.Remove(descriptor);
            services.AddDbContext<NotesDb>(o => o.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Dispose();
        base.Dispose(disposing);
    }
}

public sealed class GatewayFactory : WebApplicationFactory<Demo41.Gateway.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder.UseSetting("Testing", "true");
}

public class ApiTests : IClassFixture<IsolatedApiFactory>
{
    private readonly HttpClient _client;

    public ApiTests(IsolatedApiFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Post_Then_Get_Notes()
    {
        var create = await _client.PostAsJsonAsync("/api/v1/notes", new { title = "Behind YARP" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var list = await _client.GetFromJsonAsync<List<Note>>("/api/v1/notes");
        Assert.Equal("Behind YARP", Assert.Single(list!).Title);
    }
}

public class GatewayTests : IClassFixture<GatewayFactory>
{
    private readonly HttpClient _client;

    public GatewayTests(GatewayFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Health_IsGateway()
    {
        var res = await _client.GetFromJsonAsync<HealthDto>("/health");
        Assert.Equal("gateway", res!.Role);
    }

    private sealed record HealthDto(string Status, string Role);
}
