using System.Net;
using System.Net.Http.Json;
using Demo62;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo62.Tests;

public class MediatorTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MediatorTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Send_CreateThenList()
    {
        var create = await _client.PostAsJsonAsync("/api/v1/notes", new { title = "Via mediator" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var list = await _client.GetFromJsonAsync<List<Note>>("/api/v1/notes");
        Assert.Contains(list!, n => n.Title == "Via mediator");
    }
}
