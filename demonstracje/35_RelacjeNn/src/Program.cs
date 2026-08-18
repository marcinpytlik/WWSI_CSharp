using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("App") ?? "Data Source=demo35.db";
builder.Services.AddDbContext<CampusDb>(o => o.UseSqlite(cs));
var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampusDb>();
    await db.Database.EnsureCreatedAsync();
}

app.MapPost("/api/v1/students", async (CreateNamed dto, CampusDb db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 2)
        return Results.BadRequest(new { error = "Name must have at least 2 characters." });
    var student = new Student { Name = dto.Name.Trim() };
    db.Students.Add(student);
    await db.SaveChangesAsync();
    return Results.Created($"/api/v1/students/{student.Id}", student);
});

app.MapPost("/api/v1/courses", async (CreateNamed dto, CampusDb db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 2)
        return Results.BadRequest(new { error = "Name must have at least 2 characters." });
    var course = new Course { Title = dto.Name.Trim() };
    db.Courses.Add(course);
    await db.SaveChangesAsync();
    return Results.Created($"/api/v1/courses/{course.Id}", course);
});

app.MapPost("/api/v1/students/{studentId:int}/courses/{courseId:int}", async (int studentId, int courseId, CampusDb db) =>
{
    var student = await db.Students.Include(s => s.Courses).FirstOrDefaultAsync(s => s.Id == studentId);
    var course = await db.Courses.FindAsync(courseId);
    if (student is null || course is null) return Results.NotFound();
    if (student.Courses.Any(c => c.Id == courseId))
        return Results.Conflict(new { error = "Already enrolled." });
    student.Courses.Add(course);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/api/v1/students/{id:int}", async (int id, CampusDb db) =>
{
    var student = await db.Students.AsNoTracking()
        .Include(s => s.Courses)
        .FirstOrDefaultAsync(s => s.Id == id);
    return student is null
        ? Results.NotFound()
        : Results.Ok(new
        {
            student.Id,
            student.Name,
            courses = student.Courses.Select(c => new { c.Id, c.Title }).ToList()
        });
});

app.Run();

public sealed record CreateNamed(string Name);

public sealed class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Course> Courses { get; set; } = [];
}

public sealed class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public List<Student> Students { get; set; } = [];
}

public sealed class CampusDb : DbContext
{
    public CampusDb(DbContextOptions<CampusDb> options) : base(options) { }
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>()
            .HasMany(s => s.Courses)
            .WithMany(c => c.Students);
        modelBuilder.Entity<Student>().Property(s => s.Name).HasMaxLength(80).IsRequired();
        modelBuilder.Entity<Course>().Property(c => c.Title).HasMaxLength(120).IsRequired();
    }
}

public partial class Program;
