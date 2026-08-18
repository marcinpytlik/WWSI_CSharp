using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo54.Tests;

public sealed class IsolatedApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("RateLimit:Permit", "2");
        builder.UseSetting("RateLimit:WindowSeconds", "60");
    }
}

public class RateLimitTests : IClassFixture<IsolatedApiFactory>
{
    private readonly HttpClient _client;

    public RateLimitTests(IsolatedApiFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task ThirdRequest_Is429()
    {
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/api/v1/ping")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/api/v1/ping")).StatusCode);
        Assert.Equal(HttpStatusCode.TooManyRequests, (await _client.GetAsync("/api/v1/ping")).StatusCode);
    }
}
