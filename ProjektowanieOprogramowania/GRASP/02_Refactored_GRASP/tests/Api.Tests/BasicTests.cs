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
    public async Task CreateOrder_Vip_AppliesDiscountAndVat()
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/api/v1/orders", new
        {
            email = "a@b",
            isVip = true,
            lines = new[] { new { sku = "SKU1", qty = 2, unitPrice = 50m } }
        });

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        var body = await res.Content.ReadFromJsonAsync<CreatedOrder>();
        Assert.NotNull(body);
        // 2 * 50 = 100; VIP -10% = 90; VAT 23% = 110.70
        Assert.Equal(110.70m, body!.Final);
    }

    [Fact]
    public async Task CreateOrder_Regular_AppliesVatOnly()
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/api/v1/orders", new
        {
            email = "a@b",
            isVip = false,
            lines = new[] { new { sku = "SKU1", qty = 1, unitPrice = 100m } }
        });

        var body = await res.Content.ReadFromJsonAsync<CreatedOrder>();
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        Assert.Equal(123.00m, body!.Final);
    }

    [Fact]
    public async Task CreateOrder_BadLine_Returns400()
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/api/v1/orders", new
        {
            email = "a@b",
            isVip = false,
            lines = new[] { new { sku = "SKU1", qty = 0, unitPrice = 50m } }
        });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    private sealed record CreatedOrder(Guid Id, decimal Final);
}
