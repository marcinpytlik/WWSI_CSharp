using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo25.Tests;

public class IdempotencyTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public IdempotencyTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task SameKey_ReturnsSameOrder()
    {
        var first = await Post("key-1", "SKU-1", 2);
        var second = await Post("key-1", "SKU-1", 99);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        var a = await first.Content.ReadFromJsonAsync<Order>();
        var b = await second.Content.ReadFromJsonAsync<Order>();
        Assert.Equal(a!.Id, b!.Id);
        Assert.Equal(2, b.Qty);
    }

    [Fact]
    public async Task MissingKey_400()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/orders", new { sku = "SKU-1", qty = 1 });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    private Task<HttpResponseMessage> Post(string key, string sku, int qty)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, "/api/v1/orders")
        {
            Content = JsonContent.Create(new { sku, qty })
        };
        req.Headers.Add("Idempotency-Key", key);
        return _client.SendAsync(req);
    }
}
