using System.Net;
using System.Net.Http.Json;
using System.Text;
using Demo40;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demo40.Tests;

public sealed class IsolatedApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        builder.UseSetting("Testing", "true");
        builder.ConfigureServices(services =>
        {
            foreach (var descriptor in services.Where(d =>
                         d.ServiceType == typeof(FilesDb) ||
                         d.ServiceType == typeof(DbContextOptions<FilesDb>)).ToList())
                services.Remove(descriptor);
            services.AddDbContext<FilesDb>(o => o.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Dispose();
        base.Dispose(disposing);
    }
}

public class UploadTests : IClassFixture<IsolatedApiFactory>
{
    private readonly HttpClient _client;

    public UploadTests(IsolatedApiFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Upload_ThenDownload_UsesFakeBlobStore()
    {
        using var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("hello wwsi"))
        {
            Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain") }
        }, "file", "hello.txt");

        var upload = await _client.PostAsync("/api/v1/files", content);
        Assert.Equal(HttpStatusCode.Created, upload.StatusCode);

        var list = await _client.GetFromJsonAsync<List<FileDto>>("/api/v1/files");
        var saved = Assert.Single(list!);
        Assert.Equal("hello.txt", saved.FileName);

        var download = await _client.GetAsync($"/api/v1/files/{saved.Id}");
        download.EnsureSuccessStatusCode();
        Assert.Equal("hello wwsi", await download.Content.ReadAsStringAsync());
    }

    private sealed record FileDto(int Id, string FileName, string ContentType, long Size);
}
