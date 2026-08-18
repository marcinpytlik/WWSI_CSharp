using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("App")
         ?? "Data Source=demo23.db";
builder.Services.AddDbContext<TaskDb>(o => o.UseSqlite(cs));
var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskDb>();
    await db.Database.EnsureCreatedAsync();
}

app.MapGet("/api/v1/tasks", async (TaskDb db) =>
    await db.Items.AsNoTracking().OrderBy(t => t.Title).ToListAsync());

app.MapPost("/api/v1/tasks", async (CreateTask dto, TaskDb db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Trim().Length < 3)
        return Results.BadRequest(new { error = "Title must have at least 3 characters." });
    var item = new TaskItem { Title = dto.Title.Trim() };
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/api/v1/tasks/{item.Id}", item);
});

app.Run();

public sealed class TaskDb : DbContext
{
    public TaskDb(DbContextOptions<TaskDb> options) : base(options) { }
    public DbSet<TaskItem> Items => Set<TaskItem>();
}

public sealed class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
}

public sealed record CreateTask(string Title);
public partial class Program;
