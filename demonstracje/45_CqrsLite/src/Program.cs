using Demo45;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<NoteStore>();
builder.Services.AddSingleton<ICommandHandler<CreateNoteCommand, Note>, CreateNoteHandler>();
builder.Services.AddSingleton<IQueryHandler<ListNotesQuery, IReadOnlyList<Note>>, ListNotesHandler>();
var app = builder.Build();

app.MapPost("/api/v1/notes", async (CreateNoteCommand command, ICommandHandler<CreateNoteCommand, Note> handler) =>
{
    if (string.IsNullOrWhiteSpace(command.Title) || command.Title.Trim().Length < 3)
        return Results.BadRequest(new { error = "Title must have at least 3 characters." });
    var note = await handler.Handle(command, CancellationToken.None);
    return Results.Created($"/api/v1/notes/{note.Id}", note);
});

app.MapGet("/api/v1/notes", async (IQueryHandler<ListNotesQuery, IReadOnlyList<Note>> handler) =>
    Results.Ok(await handler.Handle(new ListNotesQuery(), CancellationToken.None)));

app.Run();

public partial class Program;
