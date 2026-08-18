using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo07.Tests;

public class NotesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public NotesApiTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Post_Then_Get_Works()
    {
        var create = await _client.PostAsJsonAsync("/api/v1/notes", new { title = "Lab notes" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var note = await create.Content.ReadFromJsonAsync<CreatedNote>();
        var get = await _client.GetAsync($"/api/v1/notes/{note!.Id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
    }

    [Fact]
    public async Task ShortTitle_Returns400()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/notes", new { title = "ab" });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    private sealed record CreatedNote(Guid Id, string Title);
}
