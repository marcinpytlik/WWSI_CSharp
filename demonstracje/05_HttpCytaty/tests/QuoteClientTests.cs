using System.Net;
using System.Text;
using Demo05;
using Xunit;

namespace Demo05.Tests;

public sealed class FakeHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _status;
    private readonly string _body;

    public FakeHandler(HttpStatusCode status, string body)
    {
        _status = status;
        _body = body;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(new HttpResponseMessage(_status)
        {
            Content = new StringContent(_body, Encoding.UTF8, "application/json")
        });
}

public class QuoteClientTests
{
    [Fact]
    public async Task GetAsync_DeserializesQuote()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, """{"text":"Hello","author":"Ada"}""");
        var client = new QuoteClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });
        var quote = await client.GetAsync();
        Assert.Equal("Hello", quote.Text);
        Assert.Equal("Ada", quote.Author);
    }

    [Fact]
    public async Task GetAsync_500_Throws()
    {
        var handler = new FakeHandler(HttpStatusCode.InternalServerError, "{}");
        var client = new QuoteClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync());
    }
}
