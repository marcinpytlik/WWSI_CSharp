using Xunit;
using System.Net;
using SharedTesting;

namespace Task11_Fetch5Parallel.Tests;

public sealed class ParallelFetcherTests
{
    [Fact]
    public async Task FetchFirst5Async_Returns5()
    {
        var http = FakeHttpMessageHandler.CreateTextResponder(uri => uri?.AbsolutePath ?? "", HttpStatusCode.OK);
        var urls = Enumerable.Range(1, 10).Select(i => $"https://example.local/{i}");

        var results = await Task11_Fetch5Parallel.ParallelFetcher.FetchFirst5Async(http, urls);

        Assert.Equal(5, results.Count);
        Assert.Equal("/1", results[0]);
        Assert.Equal("/5", results[4]);
    }
}
