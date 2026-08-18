using Demo41;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var testing = builder.Configuration.GetValue("Testing", false);

if (!testing)
{
    var cs = builder.Configuration.GetConnectionString("App")
             ?? throw new InvalidOperationException("Missing ConnectionStrings:App");
    builder.Services.AddDbContext<NotesDb>(o => o.UseSqlServer(cs));
}

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotesDb>();
    await SqlRetry.WaitAsync(() => db.Database.EnsureCreatedAsync(), logger);
}

app.MapGet("/health", () => Results.Ok(new { status = "ok", role = "api" }));

app.MapGet("/api/v1/notes", async (NotesDb db) =>
    await db.Notes.AsNoTracking().OrderBy(n => n.Id).ToListAsync());

app.MapPost("/api/v1/notes", async (CreateNote dto, NotesDb db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Trim().Length < 3)
        return Results.BadRequest(new { error = "Title must have at least 3 characters." });
    var note = new Note { Title = dto.Title.Trim() };
    db.Notes.Add(note);
    await db.SaveChangesAsync();
    return Results.Created($"/api/v1/notes/{note.Id}", note);
});

app.Run();

public sealed record CreateNote(string Title);
public partial class Program;
