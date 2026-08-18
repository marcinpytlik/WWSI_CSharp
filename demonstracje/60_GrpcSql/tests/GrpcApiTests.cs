using System.Net;
using System.Net.Http.Json;
using Demo60;
using Demo60.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demo60.Tests;

public sealed class IsolatedGrpcFactory : WebApplicationFactory<Demo60.Grpc.Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        builder.UseSetting("Testing", "true");
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

public sealed class FakeNotesClient : INotesClient
{
    private readonly List<NoteReply> _notes = [];
    public Task<NoteReply> AddAsync(string title, CancellationToken cancellationToken)
    {
        var note = new NoteReply { Id = _notes.Count + 1, Title = title.Trim() };
        _notes.Add(note);
        return Task.FromResult(note);
    }

    public Task<IReadOnlyList<NoteReply>> ListAsync(CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<NoteReply>>(_notes);
}

public sealed class IsolatedApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Testing", "true");
        builder.ConfigureServices(services => services.AddSingleton<INotesClient, FakeNotesClient>());
    }
}

public class GrpcAppTests : IClassFixture<IsolatedGrpcFactory>
{
    private readonly IsolatedGrpcFactory _factory;

    public GrpcAppTests(IsolatedGrpcFactory factory) => _factory = factory;

    [Fact]
    public async Task NotesApp_Persists()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var app = scope.ServiceProvider.GetRequiredService<NotesApp>();
        await app.AddAsync("gRPC note", CancellationToken.None);
        var list = await app.ListAsync(CancellationToken.None);
        Assert.Equal("gRPC note", Assert.Single(list).Title);
    }
}

public class HttpApiTests : IClassFixture<IsolatedApiFactory>
{
    private readonly HttpClient _client;

    public HttpApiTests(IsolatedApiFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task HttpAdapter_UsesClient()
    {
        var create = await _client.PostAsJsonAsync("/api/v1/notes", new { title = "Through HTTP" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var list = await _client.GetFromJsonAsync<List<NoteReply>>("/api/v1/notes");
        Assert.Equal("Through HTTP", Assert.Single(list!).Title);
    }
}
