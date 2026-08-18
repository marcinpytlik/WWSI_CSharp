using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demo35.Tests;

public sealed class IsolatedApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        builder.ConfigureServices(services =>
        {
            foreach (var descriptor in services.Where(d =>
                         d.ServiceType == typeof(CampusDb) ||
                         d.ServiceType == typeof(DbContextOptions<CampusDb>)).ToList())
                services.Remove(descriptor);
            services.AddDbContext<CampusDb>(o => o.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Dispose();
        base.Dispose(disposing);
    }
}

public class ManyToManyTests : IClassFixture<IsolatedApiFactory>
{
    private readonly HttpClient _client;

    public ManyToManyTests(IsolatedApiFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task StudentCanEnrollInTwoCourses()
    {
        var studentRes = await _client.PostAsJsonAsync("/api/v1/students", new { name = "Ada" });
        Assert.Equal(HttpStatusCode.Created, studentRes.StatusCode);
        var student = await studentRes.Content.ReadFromJsonAsync<NamedDto>();

        var c1 = await (await _client.PostAsJsonAsync("/api/v1/courses", new { name = "C#" })).Content.ReadFromJsonAsync<NamedDto>();
        var c2 = await (await _client.PostAsJsonAsync("/api/v1/courses", new { name = "SQL" })).Content.ReadFromJsonAsync<NamedDto>();

        var e1 = await _client.PostAsync($"/api/v1/students/{student!.Id}/courses/{c1!.Id}", null);
        var e2 = await _client.PostAsync($"/api/v1/students/{student.Id}/courses/{c2!.Id}", null);
        Assert.Equal(HttpStatusCode.NoContent, e1.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, e2.StatusCode);

        var details = await _client.GetFromJsonAsync<StudentDto>($"/api/v1/students/{student.Id}");
        Assert.Equal(2, details!.Courses.Count);
        Assert.Contains(details.Courses, c => c.Title == "C#");
        Assert.Contains(details.Courses, c => c.Title == "SQL");
    }

    [Fact]
    public async Task DuplicateEnroll_409()
    {
        var student = await (await _client.PostAsJsonAsync("/api/v1/students", new { name = "Alan" })).Content.ReadFromJsonAsync<NamedDto>();
        var course = await (await _client.PostAsJsonAsync("/api/v1/courses", new { name = "LINQ" })).Content.ReadFromJsonAsync<NamedDto>();
        await _client.PostAsync($"/api/v1/students/{student!.Id}/courses/{course!.Id}", null);
        var again = await _client.PostAsync($"/api/v1/students/{student.Id}/courses/{course.Id}", null);
        Assert.Equal(HttpStatusCode.Conflict, again.StatusCode);
    }

    private sealed record NamedDto(int Id, string? Name, string? Title);
    private sealed record CourseDto(int Id, string Title);
    private sealed record StudentDto(int Id, string Name, List<CourseDto> Courses);
}
