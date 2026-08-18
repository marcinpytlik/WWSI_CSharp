using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demo23.Tests;

public sealed class IsolatedApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        builder.ConfigureServices(services =>
        {
            foreach (var descriptor in services.Where(d =>
                         d.ServiceType == typeof(TaskDb) ||
                         d.ServiceType == typeof(DbContextOptions<TaskDb>)).ToList())
                services.Remove(descriptor);
            services.AddDbContext<TaskDb>(o => o.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Dispose();
        base.Dispose(disposing);
    }
}

public class IntegrationTests : IClassFixture<IsolatedApiFactory>
{
    private readonly HttpClient _client;

    public IntegrationTests(IsolatedApiFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Post_Then_Get_IsolatedDatabase()
    {
        var create = await _client.PostAsJsonAsync("/api/v1/tasks", new { title = "Write tests" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var list = await _client.GetFromJsonAsync<List<TaskItem>>("/api/v1/tasks");
        Assert.Single(list!);
        Assert.Equal("Write tests", list[0].Title);
    }

    [Fact]
    public async Task ShortTitle_400()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/tasks", new { title = "ab" });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }
}
