using System.Collections.Concurrent;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinimalApiDemo.Models;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// --- In-memory store ---
var users = new ConcurrentDictionary<int, User>();
int nextId = 0;

// Seed (optional)
users[Interlocked.Increment(ref nextId)] = new User { Id = nextId, Username = "sqlmaniak", Email = "contact@example.com" };

// --- Endpoints ---

// Health
app.MapGet("/health", () => Results.Json(new { status = "OK" }))
   .WithName("Health")
   .Produces(StatusCodes.Status200OK);

// Hello
app.MapGet("/hello/{name}", (string name) => Results.Ok(new { message = $"Hello, {name}!" }))
   .WithName("Hello")
   .Produces(StatusCodes.Status200OK);

// Sample (kept for reference)
app.MapGet("/sample-user", () => Results.Ok(new User { Id = 1, Username = "demo", Email = "demo@example.com" }))
   .WithName("SampleUser")
   .Produces<User>(StatusCodes.Status200OK);

// --- Users API ---

// GET /users - list all
app.MapGet("/users", () =>
{
    return Results.Ok(users.Values.OrderBy(u => u.Id));
})
.WithName("GetUsers")
.Produces<IEnumerable<User>>(StatusCodes.Status200OK);

// POST /users - create
app.MapPost("/users", (UserCreateDto dto) =>
{
    // Basic validation
    if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email))
        return Results.BadRequest(new { error = "Username and Email are required." });

    var id = Interlocked.Increment(ref nextId);
    var user = new User
    {
        Id = id,
        Username = dto.Username.Trim(),
        Email = dto.Email.Trim()
    };
    if (!users.TryAdd(user.Id, user))
        return Results.StatusCode(StatusCodes.Status500InternalServerError);

    return Results.Created($"/users/{user.Id}", user);
})
.WithName("CreateUser")
.Accepts<UserCreateDto>("application/json")
.Produces<User>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

// (Optional) GET /users/{id}
app.MapGet("/users/{id:int}", (int id) =>
{
    if (users.TryGetValue(id, out var user))
        return Results.Ok(user);
    return Results.NotFound();
})
.WithName("GetUserById")
.Produces<User>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.Run();
