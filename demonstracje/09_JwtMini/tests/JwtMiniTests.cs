using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo09.Tests;

public class JwtMiniTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public JwtMiniTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Me_WithoutToken_Is401()
    {
        var res = await _client.GetAsync("/api/v1/me");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Register_Login_Me_Works()
    {
        var email = $"ada-{Guid.NewGuid():N}@wwsi.edu.pl";
        var register = await _client.PostAsJsonAsync("/api/v1/auth/register", new { email, password = "secret1" });
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);

        var login = await _client.PostAsJsonAsync("/api/v1/auth/login", new { email, password = "secret1" });
        login.EnsureSuccessStatusCode();
        var body = await login.Content.ReadFromJsonAsync<TokenDto>();
        Assert.False(string.IsNullOrWhiteSpace(body!.AccessToken));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body.AccessToken);
        var me = await _client.GetAsync("/api/v1/me");
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
    }

    private sealed record TokenDto(string AccessToken);
}
