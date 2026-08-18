using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo53.Tests;

public class VersioningTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public VersioningTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task V1_And_V2_Differ()
    {
        var v1 = await _client.GetFromJsonAsync<V1>("/api/v1/hello");
        var v2 = await _client.GetFromJsonAsync<V2>("/api/v2/hello");
        Assert.Equal(1, v1!.Version);
        Assert.Equal("hello", v1.Message);
        Assert.Equal(2, v2!.Version);
        Assert.Equal("cześć", v2.Message);
        Assert.Equal("pl", v2.Lang);
    }

    private sealed record V1(int Version, string Message);
    private sealed record V2(int Version, string Message, string Lang);
}
