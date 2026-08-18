using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class RefactoredTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public RefactoredTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Health_Ok()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/v1/health");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_Valid_Vip_Returns201()
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/api/v1/orders", new {
            email = "a@b", isVip = true,
            lines = new[] { new { sku="SKU1", qty=2, unitPrice=50m } }
        });
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_BadLine_Returns400()
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/api/v1/orders", new {
            email = "a@b", isVip = false,
            lines = new[] { new { sku="SKU1", qty=0, unitPrice=50m } }
        });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }
}
