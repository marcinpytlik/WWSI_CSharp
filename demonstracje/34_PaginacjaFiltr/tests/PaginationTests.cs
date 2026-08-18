using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo34.Tests;

public class PaginationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PaginationTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task SkipTake_ReturnsPageAndTotal()
    {
        var page = await _client.GetFromJsonAsync<PageDto>("/api/v1/products?skip=1&take=2");
        Assert.Equal(5, page!.Total);
        Assert.Equal(1, page.Skip);
        Assert.Equal(2, page.Take);
        Assert.Equal(2, page.Items.Count);
        Assert.Equal("Notebook", page.Items[0].Name);
        Assert.Equal("Pen", page.Items[1].Name);
    }

    [Fact]
    public async Task FilterQ_CountsOnlyMatches()
    {
        var page = await _client.GetFromJsonAsync<PageDto>("/api/v1/products?q=notes&skip=0&take=1");
        Assert.Equal(2, page!.Total);
        Assert.Single(page.Items);
        Assert.Equal("Ada notes", page.Items[0].Name);
    }

    [Fact]
    public async Task InvalidTake_400()
    {
        var res = await _client.GetAsync("/api/v1/products?take=0");
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    private sealed record ItemDto(int Id, string Name);
    private sealed record PageDto(List<ItemDto> Items, int Total, int Skip, int Take);
}
