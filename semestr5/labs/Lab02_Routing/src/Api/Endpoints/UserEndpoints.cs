using Api.Domain;
using Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUsers(this RouteGroupBuilder api)
    {
        var users = api.MapGroup("/users");

        // GET /api/v1/users?name=xxx
        users.MapGet("/", async ([FromQuery] string? name, AppDbContext db, CancellationToken ct) =>
        {
            var q = db.Users.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(name)) q = q.Where(u => u.Username.Contains(name.Trim()));
            var list = await q.OrderBy(u => u.Id).ToListAsync(ct);
            return Results.Ok(list);
        })
        .Produces<IEnumerable<User>>(StatusCodes.Status200OK);

        // GET /api/v1/users/{id:int?}
        users.MapGet("/{id:int?}", async (int? id, AppDbContext db, CancellationToken ct) =>
        {
            if (id is null)
            {
                var top10 = await db.Users.AsNoTracking().OrderBy(u => u.Id).Take(10).ToListAsync(ct);
                return Results.Ok(top10);
            }
            var u = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            return u is null ? Results.NotFound() : Results.Ok(u);
        })
        .Produces<User>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /api/v1/users
        users.MapPost("/", async ([FromBody] UserCreateDto dto, AppDbContext db, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Trim().Length < 3)
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["Username"] = new[] { "Min length 3." } });

            var e = new User { Username = dto.Username.Trim() };
            db.Users.Add(e);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/v1/users/{e.Id}", e);
        })
        .Produces<User>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        return api;
    }

    public record UserCreateDto(string Username);
}
