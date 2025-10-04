using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class CrudTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public CrudTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Can_Create_And_Get_Product()
    {
        var client = _factory.CreateClient();
        var create = await client.PostAsJsonAsync("/api/v1/products", new { name = "Test", price = 12.34m });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var list = await client.GetAsync("/api/v1/products");
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
    }
}
