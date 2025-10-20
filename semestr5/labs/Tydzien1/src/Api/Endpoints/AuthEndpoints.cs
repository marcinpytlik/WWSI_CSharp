using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Contracts;
using Api.Infrastructure;

namespace Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static RouteGroupBuilder MapAuth(this RouteGroupBuilder group)
        {
            group.MapPost("/auth/login",
                ([FromBody] LoginRequest req, JwtService jwt) =>
                {
                    // Demo-only: hardcoded user; replace with real check or EF Core lookup
                    if (req.Username == "admin" && req.Password == "Password!123")
                    {
                        var token = jwt.IssueToken(req.Username, roles: new[] { "Admin" });
                        return Results.Ok(new LoginResponse(token));
                    }
                    return Results.Unauthorized();
                })
                .AllowAnonymous()
                .Produces<LoginResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);

            group.MapGet("/secure/ping", [Authorize] () => Results.Ok(new { pong = true }))
                 .WithName("SecurePing")
                 .Produces(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status401Unauthorized);
            return group;
        }
    }
}
