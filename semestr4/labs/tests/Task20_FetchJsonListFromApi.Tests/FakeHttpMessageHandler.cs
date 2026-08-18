using System.Net;
using System.Net.Http;
using System.Text;

namespace SharedTesting;

public sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

    public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        => _handler = handler;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(_handler(request));

    public static HttpClient CreateJsonResponder(Func<Uri?, string> jsonByUri, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handler = new FakeHttpMessageHandler(req =>
        {
            var json = jsonByUri(req.RequestUri);
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        });
        return new HttpClient(handler);
    }

    public static HttpClient CreateTextResponder(Func<Uri?, string> textByUri, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handler = new FakeHttpMessageHandler(req =>
        {
            var text = textByUri(req.RequestUri);
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(text, Encoding.UTF8, "text/plain")
            };
        });
        return new HttpClient(handler);
    }

    public static HttpClient CreateThrowing(Exception ex)
    {
        var handler = new FakeHttpMessageHandler(_ => throw ex);
        return new HttpClient(handler);
    }
}
