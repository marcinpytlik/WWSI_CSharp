using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is missing.");
var issuer = builder.Configuration["Jwt:Issuer"] ?? "demo09";
var audience = builder.Configuration["Jwt:Audience"] ?? "demo09";

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<UserStore>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/v1/auth/register", (RegisterDto dto, UserStore users) =>
{
    if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
        return Results.BadRequest(new { error = "Email and password (min 6) required." });
    if (!users.TryAdd(dto.Email, dto.Password))
        return Results.Conflict(new { error = "User exists." });
    return Results.Created("/api/v1/me", new { email = dto.Email.Trim().ToLowerInvariant() });
});

app.MapPost("/api/v1/auth/login", (LoginDto dto, UserStore users) =>
{
    if (!users.Verify(dto.Email, dto.Password))
        return Results.Unauthorized();

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, dto.Email.Trim().ToLowerInvariant()),
        new Claim(JwtRegisteredClaimNames.Email, dto.Email.Trim().ToLowerInvariant())
    };
    var creds = new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: creds);
    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { accessToken = jwt });
});

app.MapGet("/api/v1/me", (ClaimsPrincipal user) =>
    Results.Ok(new { email = user.FindFirstValue(ClaimTypes.Email) ?? user.FindFirstValue(JwtRegisteredClaimNames.Email) }))
    .RequireAuthorization();

app.Run();

public sealed record RegisterDto(string Email, string Password);
public sealed record LoginDto(string Email, string Password);

public sealed class UserStore
{
    private readonly Dictionary<string, string> _hashes = new(StringComparer.OrdinalIgnoreCase);
    private readonly PasswordHasher<string> _hasher = new();
    private readonly object _gate = new();

    public bool TryAdd(string email, string password)
    {
        lock (_gate)
        {
            var key = email.Trim();
            if (_hashes.ContainsKey(key)) return false;
            _hashes[key] = _hasher.HashPassword(key, password);
            return true;
        }
    }

    public bool Verify(string email, string password)
    {
        lock (_gate)
        {
            if (!_hashes.TryGetValue(email.Trim(), out var hash)) return false;
            var result = _hasher.VerifyHashedPassword(email.Trim(), hash, password);
            return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}

public partial class Program;
