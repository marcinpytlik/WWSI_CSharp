using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo55.Tests;

public class OutputCacheTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OutputCacheTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task SecondGet_ReusesCachedBody()
    {
        var first = await _client.GetFromJsonAsync<Tick>("/api/v1/ticks");
        var second = await _client.GetFromJsonAsync<Tick>("/api/v1/ticks");
        Assert.Equal(1, first!.N);
        Assert.Equal(1, second!.N);
    }

    private sealed record Tick(int N);
}
