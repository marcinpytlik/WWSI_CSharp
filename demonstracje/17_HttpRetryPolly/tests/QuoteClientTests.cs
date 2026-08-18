using System.Net;
using System.Text;
using Demo17;
using Xunit;

namespace Demo17.Tests;

public sealed class SequenceHandler : HttpMessageHandler
{
    private readonly Queue<HttpResponseMessage> _responses;
    public int Calls { get; private set; }

    public SequenceHandler(params HttpResponseMessage[] responses)
        => _responses = new Queue<HttpResponseMessage>(responses);

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Calls++;
        return Task.FromResult(_responses.Dequeue());
    }
}

public class QuoteClientTests
{
    [Fact]
    public async Task Retries_Then_Succeeds()
    {
        var handler = new SequenceHandler(
            Fail(), Fail(),
            Ok("""{"text":"ok"}"""));
        var client = new QuoteClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });
        var quote = await client.GetAsync();
        Assert.Equal("ok", quote.Text);
        Assert.Equal(3, handler.Calls);
    }

    [Fact]
    public async Task ExhaustedRetries_Throw()
    {
        var handler = new SequenceHandler(Fail(), Fail(), Fail(), Fail());
        var client = new QuoteClient(
            new HttpClient(handler) { BaseAddress = new Uri("http://localhost") },
            QuoteRetry.Create(maxAttempts: 2));
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync());
    }

    private static HttpResponseMessage Fail()
        => new(HttpStatusCode.InternalServerError) { Content = new StringContent("{}", Encoding.UTF8, "application/json") };

    private static HttpResponseMessage Ok(string json)
        => new(HttpStatusCode.OK) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
}
