using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demo36.Tests;

public sealed class IsolatedApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
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

public class ConcurrencyTests : IClassFixture<IsolatedApiFactory>
{
    private readonly HttpClient _client;

    public ConcurrencyTests(IsolatedApiFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task SecondPutWithStaleVersion_409()
    {
        var created = await (await _client.PostAsJsonAsync("/api/v1/products", new { name = "Notes" }))
            .Content.ReadFromJsonAsync<ProductDto>();
        Assert.Equal(1, created!.Version);

        var first = await _client.PutAsJsonAsync($"/api/v1/products/{created.Id}", new { name = "Notes v2", version = 1 });
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        var updated = await first.Content.ReadFromJsonAsync<ProductDto>();
        Assert.Equal(2, updated!.Version);

        var stale = await _client.PutAsJsonAsync($"/api/v1/products/{created.Id}", new { name = "Notes v3", version = 1 });
        Assert.Equal(HttpStatusCode.Conflict, stale.StatusCode);
    }

    private sealed record ProductDto(int Id, string Name, int Version);
}
