using System.Collections.Concurrent;

var notes = new ConcurrentDictionary<Guid, Note>();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/v1/notes", () => notes.Values.OrderBy(n => n.CreatedUtc));

app.MapGet("/api/v1/notes/{id:guid}", (Guid id) =>
    notes.TryGetValue(id, out var note) ? Results.Ok(note) : Results.NotFound());

app.MapPost("/api/v1/notes", (CreateNote dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Trim().Length < 3)
        return Results.BadRequest(new { error = "Title must have at least 3 characters." });

    var note = new Note(Guid.NewGuid(), dto.Title.Trim(), DateTime.UtcNow);
    notes[note.Id] = note;
    return Results.Created($"/api/v1/notes/{note.Id}", note);
});

app.MapDelete("/api/v1/notes/{id:guid}", (Guid id)
    => notes.TryRemove(id, out _) ? Results.NoContent() : Results.NotFound());

app.Run();

public sealed record CreateNote(string Title);
public sealed record Note(Guid Id, string Title, DateTime CreatedUtc);

public partial class Program;
