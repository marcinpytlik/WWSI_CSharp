using System.Diagnostics;
using System.Globalization;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using QueueCacheDemo.Api.Middleware;
using QueueCacheDemo.Contracts;
using QueueCacheDemo.Data;
using QueueCacheDemo.Entities;

var builder = WebApplication.CreateBuilder(args);
var signingKey = builder.Configuration["Auth:SigningKey"]
                 ?? throw new InvalidOperationException("Auth:SigningKey required (local lab only)");
if (Encoding.UTF8.GetByteCount(signingKey) < 32)
    throw new InvalidOperationException("Auth:SigningKey must contain at least 32 UTF-8 bytes");
var issuer = builder.Configuration["Auth:Issuer"] ?? "wwsi-demo";
var audience = builder.Configuration["Auth:Audience"] ?? "wwsi-api";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
        if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
            tracing.AddOtlpExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();
        if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
            metrics.AddOtlpExporter();
    });
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = System.Threading.RateLimiting.PartitionedRateLimiter.Create<HttpContext, string>(
        context => System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
            context.User.FindFirstValue("sub") ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Sql"), sql => sql.EnableRetryOnFailure()));
builder.Services.AddStackExchangeRedisCache(o => o.Configuration = builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapGet("/health/live", () => Results.Ok(new { status = "alive" })).AllowAnonymous();
app.MapGet("/health/ready", async (AppDbContext db, IDistributedCache cache,
    IConfiguration cfg, CancellationToken ct) =>
{
    try
    {
        if (!await db.Database.CanConnectAsync(ct))
            return Results.Problem(statusCode: 503, title: "SQL Server unavailable");

        // A missing key still exercises a real Redis GET.
        await cache.GetAsync("readiness:probe", ct);
        var factory = new ConnectionFactory
        {
            HostName = cfg["Rabbit:Host"] ?? "localhost",
            UserName = cfg["Rabbit:User"] ?? "",
            Password = cfg["Rabbit:Password"] ?? ""
        };
        await using var rabbit = await factory.CreateConnectionAsync(ct);
        return Results.Ok(new { status = "ready" });
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
        app.Logger.LogWarning(ex, "Readiness dependency check failed");
        return Results.Problem(statusCode: 503, title: "Dependency unavailable");
    }
}).AllowAnonymous();

// The atomic SQL transaction writes both a durable operation and its outbox message.
// HTTP 202 means persisted/accepted by SQL; RabbitMQ publication can occur later.
app.MapPost("/products", async (CreateProductRequest request, HttpContext http,
    IServiceScopeFactory scopes, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200
        || request.Price < 0 || request.Price > 9999999999999999m ||
        decimal.Round(request.Price, 2) != request.Price)
        return Results.BadRequest(new { error = "Invalid name or price" });

    var key = http.Request.Headers["Idempotency-Key"].ToString();
    if (string.IsNullOrWhiteSpace(key) || key.Length > 128)
        return Results.BadRequest(new { error = "Idempotency-Key header required (1..128 chars)" });

    var principal = http.User.FindFirstValue("sub");
    if (string.IsNullOrWhiteSpace(principal) || principal.Length > 128)
        return Results.Unauthorized();

    var name = request.Name.Trim();
    var requestBytes = Encoding.UTF8.GetBytes(
        JsonSerializer.Serialize(new { name, price = request.Price.ToString("0.00", CultureInfo.InvariantCulture) }));
    var hash = Convert.ToHexString(SHA256.HashData(requestBytes));
    var operationId = Guid.NewGuid();
    var productId = Guid.NewGuid();

    using var strategyScope = scopes.CreateScope();
    var strategyDb = strategyScope.ServiceProvider.GetRequiredService<AppDbContext>();
    var strategy = strategyDb.Database.CreateExecutionStrategy();

    async Task<OperationStatus> PersistAsync()
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var existing = await db.OperationStatuses.AsNoTracking()
            .SingleOrDefaultAsync(x => x.PrincipalId == principal && x.IdempotencyKey == key, ct);
        if (existing is not null) return existing;

        var now = DateTime.UtcNow;
        var operation = new OperationStatus
        {
            Id = operationId, ProductId = productId, PrincipalId = principal,
            IdempotencyKey = key, RequestHash = hash,
            Status = "Pending", CreatedAtUtc = now, UpdatedAtUtc = now
        };
        var command = new CreateProduct(operationId, productId, name, request.Price, principal, Activity.Current?.Id);
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        db.OperationStatuses.Add(operation);
        db.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(), OperationId = operationId,
            Payload = JsonSerializer.Serialize(command), CreatedAtUtc = now
        });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return operation;
    }

    OperationStatus stored;
    try
    {
        stored = await strategy.ExecuteAsync(PersistAsync);
    }
    catch (DbUpdateException)
    {
        // The unique principal/key constraint resolves concurrent duplicate POST requests.
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        stored = await db.OperationStatuses.AsNoTracking()
            .SingleOrDefaultAsync(x => x.PrincipalId == principal && x.IdempotencyKey == key, ct)
            ?? throw new InvalidOperationException("Idempotency conflict could not be resolved");
    }

    if (!string.Equals(stored.RequestHash, hash, StringComparison.Ordinal))
        return Results.Conflict(new { error = "Idempotency-Key reused with different payload" });

    return Results.Accepted($"/operations/{stored.Id}",
        new { operationId = stored.Id, productId = stored.ProductId, status = stored.Status.ToLowerInvariant() });
}).RequireAuthorization();

app.MapGet("/operations/{id:guid}", async (Guid id, HttpContext http, AppDbContext db, CancellationToken ct) =>
{
    var principal = http.User.FindFirstValue("sub")!;
    var operation = await db.OperationStatuses.AsNoTracking()
        .Where(x => x.Id == id && x.PrincipalId == principal)
        .Select(x => new { operationId = x.Id, productId = x.ProductId, status = x.Status, errorCode = x.ErrorCode })
        .SingleOrDefaultAsync(ct);
    return operation is null ? Results.NotFound() : Results.Ok(operation);
}).RequireAuthorization();

app.MapGet("/products/{id:guid}", async (Guid id, AppDbContext db, IDistributedCache cache,
    HttpContext http, CancellationToken ct) =>
{
    var principal = http.User.FindFirstValue("sub")!;
    var key = $"products:{id:N}:{principal}";
    try
    {
        var cached = await cache.GetStringAsync(key, ct);
        if (cached is not null)
        {
            http.Response.Headers["X-Cache"] = "HIT";
            return Results.Content(cached, "application/json");
        }
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
        app.Logger.LogWarning(ex, "Redis unavailable; SQL fallback");
    }

    var product = await db.Products.AsNoTracking()
        .Where(x => x.Id == id && x.PrincipalId == principal)
        .Select(x => new ProductDto(x.Id, x.Name, x.Price))
        .SingleOrDefaultAsync(ct);
    http.Response.Headers["X-Cache"] = "MISS";
    if (product is null) return Results.NotFound();

    var json = JsonSerializer.Serialize(product, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    try
    {
        await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15) }, ct);
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
        app.Logger.LogWarning(ex, "Redis write failed");
    }
    return Results.Content(json, "application/json");
}).RequireAuthorization();

app.Run();
public sealed record CreateProductRequest(string Name, decimal Price);
