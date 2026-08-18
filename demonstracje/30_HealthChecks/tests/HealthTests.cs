using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo30.Tests;

public class HealthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Healthy_Returns200()
    {
        await _client.GetAsync("/api/v1/ready");
        var res = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Unhealthy_Returns503()
    {
        await _client.GetAsync("/api/v1/break");
        var res = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, res.StatusCode);
    }
}
