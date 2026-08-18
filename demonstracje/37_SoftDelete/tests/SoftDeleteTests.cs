using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demo37.Tests;

public sealed class IsolatedApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        builder.ConfigureServices(services =>
        {
            foreach (var descriptor in services.Where(d =>
                         d.ServiceType == typeof(NotesDb) ||
                         d.ServiceType == typeof(DbContextOptions<NotesDb>)).ToList())
                services.Remove(descriptor);
            services.AddDbContext<NotesDb>(o => o.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Dispose();
        base.Dispose(disposing);
    }
}

public class SoftDeleteTests : IClassFixture<IsolatedApiFactory>
{
    private readonly IsolatedApiFactory _factory;
    private readonly HttpClient _client;

    public SoftDeleteTests(IsolatedApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Delete_HidesNote_ButRowRemains()
    {
        var created = await (await _client.PostAsJsonAsync("/api/v1/notes", new { title = "Keep me" }))
            .Content.ReadFromJsonAsync<NoteDto>();

        var del = await _client.DeleteAsync($"/api/v1/notes/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);

        var get = await _client.GetAsync($"/api/v1/notes/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);

        var list = await _client.GetFromJsonAsync<List<NoteDto>>("/api/v1/notes");
        Assert.Empty(list!);

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<NotesDb>();
        var hidden = await db.Notes.IgnoreQueryFilters().SingleAsync(n => n.Id == created.Id);
        Assert.True(hidden.IsDeleted);
        Assert.Equal("Keep me", hidden.Title);
    }

    private sealed record NoteDto(int Id, string Title, bool IsDeleted);
}
