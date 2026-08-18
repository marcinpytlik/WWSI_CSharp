using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo56.Tests;

public class OpenApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OpenApiTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task OpenApiDocument_ContainsHello()
    {
        var json = await _client.GetStringAsync("/openapi/v1.json");
        Assert.Contains("/api/v1/hello", json, StringComparison.Ordinal);
    }

    [Fact]
    public async Task SwaggerUi_IsAvailable()
    {
        var res = await _client.GetAsync("/swagger/index.html");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}
