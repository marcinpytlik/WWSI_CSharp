using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo22.Tests;

public class MiddlewareTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MiddlewareTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Echoes_CorrelationId()
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "/api/v1/ok");
        req.Headers.Add("X-Correlation-Id", "cid-42");
        var res = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        Assert.Equal("cid-42", res.Headers.GetValues("X-Correlation-Id").Single());
    }

    [Fact]
    public async Task Boom_ReturnsProblem_WithCorrelationId()
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "/api/v1/boom");
        req.Headers.Add("X-Correlation-Id", "cid-err");
        var res = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.InternalServerError, res.StatusCode);
        var body = await res.Content.ReadAsStringAsync();
        Assert.Contains("cid-err", body);
        Assert.Contains("Server error", body);
    }
}
