using Xunit;
using System.Net;
using System.Net.Http;
using System.Text;
using SharedTesting;

namespace Task10_FetchApiAsync.Tests;

public sealed class ApiClientTests
{
    [Fact]
    public async Task FetchStringAsync_ReturnsBody()
    {
        var http = FakeHttpMessageHandler.CreateTextResponder(_ => "hello", HttpStatusCode.OK);

        var result = await Task10_FetchApiAsync.ApiClient.FetchStringAsync(http, "https://example.local/test");

        Assert.Equal("hello", result);
    }
}
