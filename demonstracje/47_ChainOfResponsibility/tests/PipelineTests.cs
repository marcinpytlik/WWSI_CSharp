using System.Net;
using System.Net.Http.Json;
using Demo47;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo47.Tests;

public class PipelineTests
{
    private readonly CodeHandler _pipeline = CodePipeline.Create();

    [Fact]
    public void Empty_StopsFirst() => Assert.Equal("Code is required.", _pipeline.Handle(""));

    [Fact]
    public void BadFormat_StopsSecond() => Assert.Equal("Code must match AAA-000.", _pipeline.Handle("abc"));

    [Fact]
    public void OverLimit_StopsThird() => Assert.Equal("Numeric part must be <= 500.", _pipeline.Handle("ABC-999"));

    [Fact]
    public void Valid_Passes() => Assert.Null(_pipeline.Handle("ABC-123"));
}

public class PipelineApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PipelineApiTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task ValidCode_200()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/codes", new { code = "ABC-100" });
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task InvalidCode_400()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/codes", new { code = "nope" });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }
}
