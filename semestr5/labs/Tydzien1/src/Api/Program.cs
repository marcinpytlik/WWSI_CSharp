using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Api.Infrastructure;
using Api.Infrastructure.Middleware;
using Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Config
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// DbContext (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Sql")));

// JWT Auth
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"] ?? "super-secret-dev-key-change");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
builder.Services.AddAuthorization();

// Infra services
builder.Services.AddSingleton<JwtService>();

var app = builder.Build();

// Kestrel basic config could be adjusted via appsettings; defaults are OK for lab.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

app.UseAuthentication();
app.UseAuthorization();

// Middleware: Correlation Id
app.UseMiddleware<CorrelationIdMiddleware>();

// API grouping & versioning in path
var api = app.MapGroup("/api/v1");
api.MapHealth();
api.MapAuth();
api.MapProducts();

app.Run();

// Make Program visible for WebApplicationFactory in tests
public partial class Program { }
