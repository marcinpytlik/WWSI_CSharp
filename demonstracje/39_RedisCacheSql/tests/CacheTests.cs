using System.Net;
using System.Net.Http.Json;
using Demo39;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demo39.Tests;

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
                         d.ServiceType == typeof(CatalogDb) ||
                         d.ServiceType == typeof(DbContextOptions<CatalogDb>)).ToList())
                services.Remove(descriptor);
            services.AddDbContext<CatalogDb>(o => o.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Dispose();
        base.Dispose(disposing);
    }
}

public class CacheTests : IClassFixture<IsolatedApiFactory>
{
    private readonly HttpClient _client;

    public CacheTests(IsolatedApiFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Post_InvalidatesCache_AndListGrows()
    {
        var empty = await _client.GetFromJsonAsync<List<Product>>("/api/v1/products");
        Assert.Empty(empty!);

        var create = await _client.PostAsJsonAsync("/api/v1/products", new { name = "Cached notes" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var list = await _client.GetFromJsonAsync<List<Product>>("/api/v1/products");
        Assert.Single(list!);
        Assert.Equal("Cached notes", list![0].Name);
    }

    [Fact]
    public async Task ShortName_400()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/products", new { name = "x" });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }
}
