using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo33.Tests;

public class JwtRoleTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public JwtRoleTests(WebApplicationFactory<Program> factory)
        => _factory = factory;

    [Fact]
    public async Task Admin_WithoutToken_Is401()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/v1/admin/stats");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task UserRole_OnAdminEndpoint_Is403()
    {
        var client = await AuthorizeAsync("user@wwsi.edu.pl", "user123");
        var res = await client.GetAsync("/api/v1/admin/stats");
        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }

    [Fact]
    public async Task AdminRole_OnAdminEndpoint_Is200()
    {
        var client = await AuthorizeAsync("admin@wwsi.edu.pl", "admin123");
        var res = await client.GetAsync("/api/v1/admin/stats");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task User_CanReadMe()
    {
        var client = await AuthorizeAsync("user@wwsi.edu.pl", "user123");
        var me = await client.GetFromJsonAsync<MeDto>("/api/v1/me");
        Assert.Equal("User", me!.Role);
    }

    private async Task<HttpClient> AuthorizeAsync(string email, string password)
    {
        var client = _factory.CreateClient();
        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password });
        login.EnsureSuccessStatusCode();
        var body = await login.Content.ReadFromJsonAsync<TokenDto>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.AccessToken);
        return client;
    }

    private sealed record TokenDto(string AccessToken);
    private sealed record MeDto(string Email, string Role);
}
