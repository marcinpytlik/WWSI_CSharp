using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class BasicTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BasicTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task CreateOrder_Vip_Returns201AndDiscountedGross()
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/orders", new { email = "a@b", basePrice = 100m, isVip = true });
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        var body = await res.Content.ReadFromJsonAsync<CreatedOrder>();
        Assert.Equal(110.70m, body!.Final);
    }

    [Fact]
    public async Task CreateOrder_BadEmail_Returns400()
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/orders", new { email = "bad", basePrice = 100m, isVip = false });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    private sealed record CreatedOrder(Guid Id, decimal Final);
}
