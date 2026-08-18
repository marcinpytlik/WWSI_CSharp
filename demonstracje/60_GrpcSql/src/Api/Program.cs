using Demo60.Api;

var builder = WebApplication.CreateBuilder(args);
var testing = builder.Configuration.GetValue("Testing", false);
if (!testing)
    builder.Services.AddSingleton<INotesClient, GrpcNotesClient>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok", role = "api" }));

app.MapPost("/api/v1/notes", async (CreateNote dto, INotesClient client, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Trim().Length < 3)
        return Results.BadRequest(new { error = "Title must have at least 3 characters." });
    var note = await client.AddAsync(dto.Title, cancellationToken);
    return Results.Created($"/api/v1/notes/{note.Id}", note);
});

app.MapGet("/api/v1/notes", async (INotesClient client, CancellationToken cancellationToken) =>
    Results.Ok(await client.ListAsync(cancellationToken)));

app.Run();

public sealed record CreateNote(string Title);
public partial class Program;
