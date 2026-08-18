using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo38.Tests;

public class FluentValidationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public FluentValidationTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Valid_Returns201()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/products", new { sku = "SKU-1", name = "Notes", price = 9.9 });
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
    }

    [Fact]
    public async Task InvalidSku_Returns400Problem()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/products", new { sku = "x", name = "Notes", price = 9.9 });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        var body = await res.Content.ReadAsStringAsync();
        Assert.Contains("Sku", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task NegativePrice_Returns400()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/products", new { sku = "SKU-2", name = "Notes", price = -1 });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }
}
