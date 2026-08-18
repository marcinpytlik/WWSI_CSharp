using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is missing.");
var issuer = builder.Configuration["Jwt:Issuer"] ?? "demo33";
var audience = builder.Configuration["Jwt:Audience"] ?? "demo33";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = ClaimTypes.Role
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<UserStore>();

var app = builder.Build();
app.Services.GetRequiredService<UserStore>().SeedDemoUsers();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/v1/auth/login", (LoginDto dto, UserStore users) =>
{
    if (!users.TryGetRole(dto.Email, dto.Password, out var role))
        return Results.Unauthorized();

    var email = dto.Email.Trim().ToLowerInvariant();
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(JwtRegisteredClaimNames.Email, email),
        new Claim(ClaimTypes.Role, role)
    };
    var creds = new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: creds);
    return Results.Ok(new { accessToken = new JwtSecurityTokenHandler().WriteToken(token), role });
});

app.MapGet("/api/v1/me", (ClaimsPrincipal user) =>
    Results.Ok(new
    {
        email = user.FindFirstValue(ClaimTypes.Email) ?? user.FindFirstValue(JwtRegisteredClaimNames.Email),
        role = user.FindFirstValue(ClaimTypes.Role)
    }))
    .RequireAuthorization();

app.MapGet("/api/v1/admin/stats", () => Results.Ok(new { users = 2, role = "Admin" }))
    .RequireAuthorization(policy => policy.RequireRole("Admin"));

app.Run();

public sealed record LoginDto(string Email, string Password);

public sealed class UserStore
{
    private readonly Dictionary<string, (string Hash, string Role)> _users = new(StringComparer.OrdinalIgnoreCase);
    private readonly PasswordHasher<string> _hasher = new();
    private readonly object _gate = new();

    public void SeedDemoUsers()
    {
        TryAdd("user@wwsi.edu.pl", "user123", "User");
        TryAdd("admin@wwsi.edu.pl", "admin123", "Admin");
    }

    public bool TryAdd(string email, string password, string role)
    {
        lock (_gate)
        {
            var key = email.Trim();
            if (_users.ContainsKey(key)) return false;
            _users[key] = (_hasher.HashPassword(key, password), role);
            return true;
        }
    }

    public bool TryGetRole(string email, string password, out string role)
    {
        lock (_gate)
        {
            role = "";
            if (!_users.TryGetValue(email.Trim(), out var entry)) return false;
            var result = _hasher.VerifyHashedPassword(email.Trim(), entry.Hash, password);
            if (result is not (PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded))
                return false;
            role = entry.Role;
            return true;
        }
    }
}

public partial class Program;
