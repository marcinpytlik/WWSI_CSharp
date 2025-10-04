using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class BasicTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public BasicTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task CreateOrder_Valid_Returns201()
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/orders", new { email="a@b", basePrice=100m, isVip=true });
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_BadEmail_Returns400()
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/orders", new { email="bad", basePrice=100m, isVip=false });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }
}
