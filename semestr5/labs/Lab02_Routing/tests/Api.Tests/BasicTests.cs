using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class BasicTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _app;
    public BasicTests(WebApplicationFactory<Program> app) => _app = app;

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var c = _app.CreateClient();
        var res = await c.GetAsync("/api/v1/health");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}
