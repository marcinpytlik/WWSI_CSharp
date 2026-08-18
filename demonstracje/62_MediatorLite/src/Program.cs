using Demo62;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<NoteStore>();
builder.Services.AddSingleton<IMediator, Mediator>();
builder.Services.AddSingleton<IRequestHandler<CreateNote, Note>, CreateNoteHandler>();
builder.Services.AddSingleton<IRequestHandler<ListNotes, IReadOnlyList<Note>>, ListNotesHandler>();
var app = builder.Build();

app.MapPost("/api/v1/notes", async (CreateNote command, IMediator mediator) =>
{
    if (string.IsNullOrWhiteSpace(command.Title) || command.Title.Trim().Length < 3)
        return Results.BadRequest(new { error = "Title must have at least 3 characters." });
    var note = await mediator.Send(command);
    return Results.Created($"/api/v1/notes/{note.Id}", note);
});

app.MapGet("/api/v1/notes", async (IMediator mediator) =>
    Results.Ok(await mediator.Send(new ListNotes())));

app.Run();

public partial class Program;
