using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core
var conn = builder.Configuration.GetConnectionString("Sql");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(conn));

// JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev_key_change_me";
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// Health
app.MapGet("/api/v1/health", () => Results.Ok(new { status = "ok" }))
   .WithName("Health")
   .WithOpenApi();

// Example secured endpoint
app.MapGet("/api/v1/secure/ping", () => Results.Ok(new { pong = true }))
   .RequireAuthorization()
   .WithOpenApi();

// Basic CRUD for Product (in-memory bootstrap via EF context)
app.MapGet("/api/v1/products", async (AppDbContext db) =>
    Results.Ok(await db.Products.AsNoTracking().ToListAsync()))
   .WithOpenApi();

app.MapPost("/api/v1/products", async (AppDbContext db, ProductDto dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name)) return Results.BadRequest(new { error = "Name required" });
    var p = new Product { Name = dto.Name, Price = dto.Price };
    db.Products.Add(p);
    await db.SaveChangesAsync();
    return Results.Created($"/api/v1/products/{p.Id}", p);
}).WithOpenApi();

app.MapGet("/api/v1/products/{id:int}", async (AppDbContext db, int id) =>
{
    var p = await db.Products.FindAsync(id);
    return p is null ? Results.NotFound() : Results.Ok(p);
}).WithOpenApi();

app.MapPut("/api/v1/products/{id:int}", async (AppDbContext db, int id, ProductDto dto) =>
{
    var p = await db.Products.FindAsync(id);
    if (p is null) return Results.NotFound();
    p.Name = dto.Name; p.Price = dto.Price;
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithOpenApi();

app.MapDelete("/api/v1/products/{id:int}", async (AppDbContext db, int id) =>
{
    var p = await db.Products.FindAsync(id);
    if (p is null) return Results.NotFound();
    db.Products.Remove(p);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithOpenApi();

app.Run();

public partial class Program { } // for WebApplicationFactory

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Price).HasPrecision(18,2);
        });
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public record ProductDto(string Name, decimal Price);
