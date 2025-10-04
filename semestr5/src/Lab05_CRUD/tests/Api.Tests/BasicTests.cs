using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class BasicTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public BasicTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/v1/health");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Secure_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/v1/secure/ping");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }
}
