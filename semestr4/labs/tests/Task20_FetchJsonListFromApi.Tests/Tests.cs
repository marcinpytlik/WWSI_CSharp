using Xunit;
using SharedTesting;

namespace Task20_FetchJsonListFromApi.Tests;

public sealed record Item(int Id, string Name);

public sealed class JsonListFetcherTests
{
    [Fact]
    public async Task FetchListAsync_DeserializesList()
    {
        var http = FakeHttpMessageHandler.CreateJsonResponder(_ => """[{"id":1,"name":"A"},{"id":2,"name":"B"}]""");
        var list = await Task20_FetchJsonListFromApi.JsonListFetcher.FetchListAsync<Item>(http, "https://example.local/items");

        Assert.Equal(2, list.Count);
        Assert.Equal("B", list[1].Name);
    }
}
