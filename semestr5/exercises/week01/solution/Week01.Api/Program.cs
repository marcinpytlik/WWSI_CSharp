using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<DemoOptions>(builder.Configuration.GetSection("Demo"));
builder.Services.AddSingleton<NoteStore>();

var app = builder.Build();
var api = app.MapGroup("/api/v1").WithTags("Week 01");

api.MapGet("/hello", () => TypedResults.Ok(new { message = "Witaj w Minimal API .NET 10!" }))
   .WithName("Hello");

api.MapGet("/health", () => TypedResults.Ok(new
{
    status = "healthy",
    timestampUtc = DateTimeOffset.UtcNow,
    version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown"
})).WithName("Health");

api.MapGet("/config", (Microsoft.Extensions.Options.IOptions<DemoOptions> settings) =>
    TypedResults.Ok(new ConfigResponse(settings.Value.AppName, settings.Value.PublicMessage)))
    .WithName("PublicConfiguration");

var notes = api.MapGroup("/notes").WithTags("Notes");
notes.MapGet("/", (NoteStore store) =>
    TypedResults.Ok(store.GetAll()));

notes.MapGet("/{id:int}", (int id, NoteStore store) =>
    store.Find(id) is { } note
        ? Results.Ok(note)
        : Results.NotFound());

notes.MapPost("/", (CreateNoteRequest request, NoteStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Text) || request.Text.Length > 500)
        return Results.BadRequest(new { error = "Tekst musi mieć od 1 do 500 znaków." });
    var note = store.Add(request.Text.Trim());
    return Results.Created("/api/v1/notes/" + note.Id, note);
});

notes.MapDelete("/{id:int}", (int id, NoteStore store) =>
    store.Remove(id) ? Results.NoContent() : Results.NotFound());

app.Run();

public sealed class DemoOptions
{
    public string AppName { get; set; } = "WWSI Week 01";
    public string PublicMessage { get; set; } = "Przykład ustawień publicznych";
}
public record ConfigResponse(string AppName, string PublicMessage);
public record Note(int Id, string Text);
public record CreateNoteRequest(string? Text);
public sealed class NoteStore
{
    private readonly ConcurrentDictionary<int, Note> _notes = new();
    private int _nextId;
    public Note[] GetAll() => _notes.Values.OrderBy(x => x.Id).ToArray();
    public Note? Find(int id) => _notes.TryGetValue(id, out var note) ? note : null;
    public Note Add(string text)
    {
        var note = new Note(Interlocked.Increment(ref _nextId), text);
        _notes[note.Id] = note;
        return note;
    }
    public bool Remove(int id) => _notes.TryRemove(id, out _);
}
