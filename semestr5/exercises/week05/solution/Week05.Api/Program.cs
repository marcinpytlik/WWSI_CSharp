using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Users")
    ?? throw new InvalidOperationException("Brak ConnectionStrings:Users.");
builder.Services.AddDbContext<UsersDbContext>(options => options.UseSqlite(connectionString));

var app = builder.Build();

// Wyłącznie samodzielne demo tygodnia 5. W docelowym projekcie kontynuującym
// tydzień 4 należy używać migracji EF Core (MigrateAsync), a nie EnsureCreated.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<UsersDbContext>().Database.EnsureCreatedAsync();
}

var users = app.MapGroup("/users");

users.MapPost("/", async (CreateUserDto dto, UsersDbContext db, CancellationToken ct) =>
{
    var errors = Validate(dto);
    if (errors.Count > 0) return Results.ValidationProblem(errors);
    var email = dto.Email.Trim();
    if (await db.Users.AnyAsync(x => x.Email == email, ct))
        return Results.Conflict(new { message = "Adres e-mail już istnieje." });

    var entity = new User { Name = dto.Name.Trim(), Email = email };
    db.Users.Add(entity);
    await db.SaveChangesAsync(ct);
    return Results.CreatedAtRoute("GetUser", new { id = entity.Id }, ToDto(entity));
});

users.MapGet("/", async (UsersDbContext db, string? search, string? sortBy, bool descending = false,
    int page = 1, int pageSize = 10, CancellationToken ct = default) =>
{
    if (page < 1 || pageSize is < 1 or > 100)
        return Results.BadRequest(new { message = "page >= 1 oraz pageSize od 1 do 100." });
    if (sortBy is not null && sortBy is not ("id" or "name" or "email"))
        return Results.BadRequest(new { message = "sortBy: id, name lub email." });

    IQueryable<User> query = db.Users.AsNoTracking();
    if (!string.IsNullOrWhiteSpace(search))
    {
        var term = search.Trim();
        query = query.Where(x => x.Name.Contains(term) || x.Email.Contains(term));
    }

    var total = await query.CountAsync(ct);
    query = (sortBy, descending) switch
    {
        ("name", false) => query.OrderBy(x => x.Name).ThenBy(x => x.Id),
        ("name", true) => query.OrderByDescending(x => x.Name).ThenBy(x => x.Id),
        ("email", false) => query.OrderBy(x => x.Email).ThenBy(x => x.Id),
        ("email", true) => query.OrderByDescending(x => x.Email).ThenBy(x => x.Id),
        (_, true) => query.OrderByDescending(x => x.Id),
        _ => query.OrderBy(x => x.Id)
    };
    var offset = ((long)page - 1) * pageSize;
    var items = offset >= total
        ? new List<UserDto>()
        : await query.Skip((int)offset).Take(pageSize).Select(x => new UserDto(x.Id, x.Name, x.Email)).ToListAsync(ct);
    return Results.Ok(new PagedResult<UserDto>(items, page, pageSize, total, (int)Math.Ceiling(total / (double)pageSize)));
});

users.MapGet("/{id:int}", async (int id, UsersDbContext db, CancellationToken ct) =>
{
    var user = await db.Users.AsNoTracking()
        .Where(x => x.Id == id)
        .Select(x => new UserDto(x.Id, x.Name, x.Email))
        .FirstOrDefaultAsync(ct);
    return user is null ? Results.NotFound() : Results.Ok(user);
}).WithName("GetUser");

users.MapPut("/{id:int}", async (int id, UpdateUserDto dto, UsersDbContext db, CancellationToken ct) =>
{
    var errors = Validate(dto);
    if (errors.Count > 0) return Results.ValidationProblem(errors);
    var user = await db.Users.FindAsync([id], ct);
    if (user is null) return Results.NotFound();
    var email = dto.Email.Trim();
    if (await db.Users.AnyAsync(x => x.Id != id && x.Email == email, ct))
        return Results.Conflict(new { message = "Adres e-mail już istnieje." });
    user.Name = dto.Name.Trim();
    user.Email = email;
    await db.SaveChangesAsync(ct);
    return Results.Ok(ToDto(user));
});

users.MapDelete("/{id:int}", async (int id, UsersDbContext db, CancellationToken ct) =>
{
    var user = await db.Users.FindAsync([id], ct);
    if (user is null) return Results.NotFound();
    db.Users.Remove(user);
    await db.SaveChangesAsync(ct);
    return Results.NoContent();
});

app.Run();

static UserDto ToDto(User user) => new(user.Id, user.Name, user.Email);
static Dictionary<string, string[]> Validate<T>(T dto)
{
    var results = new List<ValidationResult>();
    Validator.TryValidateObject(dto!, new ValidationContext(dto!), results, validateAllProperties: true);
    return results
        .SelectMany(result => result.MemberNames.DefaultIfEmpty("request")
            .Select(name => new { name, message = result.ErrorMessage ?? "Niepoprawna wartość." }))
        .GroupBy(x => x.name)
        .ToDictionary(g => g.Key, g => g.Select(x => x.message).ToArray());
}

public sealed class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}
public sealed record CreateUserDto(
    [property: Required, StringLength(100, MinimumLength = 2)] string Name,
    [property: Required, EmailAddress, StringLength(200)] string Email);
public sealed record UpdateUserDto(
    [property: Required, StringLength(100, MinimumLength = 2)] string Name,
    [property: Required, EmailAddress, StringLength(200)] string Email);
public sealed record UserDto(int Id, string Name, string Email);
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount, int TotalPages);

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<User>();
        user.ToTable("Users");
        user.HasKey(x => x.Id);
        user.Property(x => x.Name).HasMaxLength(100).IsRequired();
        user.Property(x => x.Email).HasMaxLength(200).IsRequired();
        user.HasIndex(x => x.Email).IsUnique();
    }
}
