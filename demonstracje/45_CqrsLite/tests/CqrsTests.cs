using System.Net;
using System.Net.Http.Json;
using Demo45;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Demo45.Tests;

public class CqrsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CqrsTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task CommandWrites_QueryReads()
    {
        var create = await _client.PostAsJsonAsync("/api/v1/notes", new { title = "CQRS note" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var list = await _client.GetFromJsonAsync<List<Note>>("/api/v1/notes");
        Assert.Contains(list!, n => n.Title == "CQRS note");
    }

    [Fact]
    public async Task HandlersAreSeparateTypes()
    {
        Assert.NotEqual(typeof(CreateNoteHandler), typeof(ListNotesHandler));
    }
}
