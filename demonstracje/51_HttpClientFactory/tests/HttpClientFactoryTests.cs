using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demo51.Tests;

public sealed class FakeHandler : HttpMessageHandler
{
    public Uri? LastUri { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastUri = request.RequestUri;
        var json = """{"text":"Talk is cheap.","author":"Linus"}""";
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
    }
}

public sealed class IsolatedApiFactory : WebApplicationFactory<Program>
{
    public FakeHandler Handler { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Quotes:BaseAddress", "https://quotes.test/");
        builder.ConfigureServices(services =>
        {
            services.AddHttpClient("quotes")
                .ConfigurePrimaryHttpMessageHandler(() => Handler);
        });
    }
}

public class HttpClientFactoryTests : IClassFixture<IsolatedApiFactory>
{
    private readonly IsolatedApiFactory _factory;
    private readonly HttpClient _client;

    public HttpClientFactoryTests(IsolatedApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task NamedClient_UsesBaseAddress()
    {
        var quote = await _client.GetFromJsonAsync<QuoteDto>("/api/v1/quote");
        Assert.Equal("Linus", quote!.Author);
        Assert.Equal("https://quotes.test/quote", _factory.Handler.LastUri!.ToString());
    }
}
