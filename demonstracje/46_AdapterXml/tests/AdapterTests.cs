using System.Net;
using System.Net.Http.Json;
using Demo46;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo46.Tests;

public class AdapterTests
{
    [Fact]
    public async Task Adapter_ParsesLegacyXml()
    {
        var adapter = new XmlQuoteAdapter(new InMemoryXmlQuoteSource());
        var quote = await adapter.GetAsync("q1", CancellationToken.None);
        Assert.Equal("Linus Torvalds", quote!.Author);
        Assert.Contains("code", quote.Text, StringComparison.OrdinalIgnoreCase);
    }
}

public class QuoteApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public QuoteApiTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task GetKnownQuote_200()
    {
        var quote = await _client.GetFromJsonAsync<Quote>("/api/v1/quotes/q1");
        Assert.Equal("q1", quote!.Id);
    }

    [Fact]
    public async Task MissingQuote_404()
    {
        var res = await _client.GetAsync("/api/v1/quotes/missing");
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }
}
