using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("App") ?? "Data Source=demo37.db";
builder.Services.AddDbContext<NotesDb>(o => o.UseSqlite(cs));
var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotesDb>();
    await db.Database.EnsureCreatedAsync();
}

app.MapPost("/api/v1/notes", async (CreateNote dto, NotesDb db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Trim().Length < 3)
        return Results.BadRequest(new { error = "Title must have at least 3 characters." });
    var note = new Note { Title = dto.Title.Trim() };
    db.Notes.Add(note);
    await db.SaveChangesAsync();
    return Results.Created($"/api/v1/notes/{note.Id}", note);
});

app.MapGet("/api/v1/notes", async (NotesDb db) =>
    await db.Notes.AsNoTracking().OrderBy(n => n.Id).ToListAsync());

app.MapGet("/api/v1/notes/{id:int}", async (int id, NotesDb db) =>
    await db.Notes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id) is { } note
        ? Results.Ok(note)
        : Results.NotFound());

app.MapDelete("/api/v1/notes/{id:int}", async (int id, NotesDb db) =>
{
    var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == id);
    if (note is null) return Results.NotFound();
    note.IsDeleted = true;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

public sealed record CreateNote(string Title);

public sealed class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public bool IsDeleted { get; set; }
}

public sealed class NotesDb : DbContext
{
    public NotesDb(DbContextOptions<NotesDb> options) : base(options) { }
    public DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>(e =>
        {
            e.Property(n => n.Title).HasMaxLength(120).IsRequired();
            e.HasQueryFilter(n => !n.IsDeleted);
        });
    }
}

public partial class Program;
