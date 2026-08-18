using System.Net;
using System.Net.Http.Json;
using Demo61;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demo61.Tests;

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
                         d.ServiceType == typeof(LibraryDb) ||
                         d.ServiceType == typeof(DbContextOptions<LibraryDb>)).ToList())
                services.Remove(descriptor);
            services.AddDbContext<LibraryDb>(o => o.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Dispose();
        base.Dispose(disposing);
    }
}

public class PostgresSwapTests : IClassFixture<IsolatedApiFactory>
{
    private readonly HttpClient _client;

    public PostgresSwapTests(IsolatedApiFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task SameEndpoints_WorkOnSqliteInTests()
    {
        var create = await _client.PostAsJsonAsync("/api/v1/books", new { title = "Clean Code", year = 2008 });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var list = await _client.GetFromJsonAsync<List<Book>>("/api/v1/books");
        Assert.Equal("Clean Code", Assert.Single(list!).Title);
    }
}
