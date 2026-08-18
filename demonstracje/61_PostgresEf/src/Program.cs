using Demo61;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var testing = builder.Configuration.GetValue("Testing", false);

if (!testing)
{
    var cs = builder.Configuration.GetConnectionString("App")
             ?? throw new InvalidOperationException("Missing ConnectionStrings:App");
    builder.Services.AddDbContext<LibraryDb>(o => o.UseNpgsql(cs));
}

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDb>();
    await SqlRetry.WaitAsync(() => db.Database.EnsureCreatedAsync(), logger);
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/v1/books", async (LibraryDb db) =>
    await db.Books.AsNoTracking().OrderBy(b => b.Title).ToListAsync());

app.MapPost("/api/v1/books", async (CreateBook dto, LibraryDb db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Trim().Length < 3)
        return Results.BadRequest(new { error = "Title must have at least 3 characters." });
    if (dto.Year < 1)
        return Results.BadRequest(new { error = "Year must be >= 1." });
    var book = new Book { Title = dto.Title.Trim(), Year = dto.Year };
    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/api/v1/books/{book.Id}", book);
});

app.Run();

public sealed record CreateBook(string Title, int Year);
public partial class Program;
