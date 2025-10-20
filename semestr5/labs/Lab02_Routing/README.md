# Lab02 Routing — Minimal API (SQL Server)

## Quick start
```pwsh
dotnet restore
dotnet build
dotnet tool install --global dotnet-ef
dotnet ef migrations add Init --project src/Api/Api.csproj --startup-project src/Api/Api.csproj
dotnet ef database update --project src/Api/Api.csproj --startup-project src/Api/Api.csproj
dotnet watch run --project src/Api/Api.csproj
```

## Endpoints
- GET  /api/v1/health
- GET  /api/v1/products?minPrice=&maxPrice=&search=&page=&pageSize=
- GET  /api/v1/products/{id:int}
- GET  /api/v1/products/sku/{sku}
- POST /api/v1/products
- PUT  /api/v1/products/{id}
- DELETE /api/v1/products/{id}
- GET  /api/v1/users?name=
- GET  /api/v1/users/{id:int?}
- POST /api/v1/users
- GET  /api/v1/routing/by-id/{id} (int wins vs string)
- GET  /api/v1/routing/from-header
- POST /api/v1/routing/accept (202), DELETE /api/v1/routing/noop (204)
  
1) Program.cs — popraw Swagger i (na razie) wyłącz JWT

Zmień nagłówek usług i middleware tak:

using Api.Endpoints;
using Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
// ✅ jawnie podaj setupAction – część wersji Swashbuckle tego wymaga
builder.Services.AddSwaggerGen(_ => { });
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Sql")));

// ❗ jeśli nie masz zainstalowanego pakietu JwtBearer zgodnego z Twoim TFM,
// to tymczasowo wyłącz autoryzację (routing lab i tak działa bez tego).
// Po doinstalowaniu pakietu – odkomentuj.
/*
using Microsoft.AspNetCore.Authentication.JwtBearer;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(_ => { });
builder.Services.AddAuthorization();
*/

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(_ => { }); // ✅ jawny setupAction
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

app.UseMiddleware<CorrelationIdMiddleware>();
// app.UseAuthentication();
// app.UseAuthorization();

var api = app.MapGroup("/api/v1");
api.MapHealth();
api.MapProducts();
api.MapUsers();
api.MapRoutingDemos();

app.Run();

public partial class Program { }

Jeśli chcesz mieć JWT teraz

Dodaj wersje pakietów w Api.csproj zgodne z Twoim SDK (przykład dla .NET 8 stabilnego; dla .NET 9 użyj odpow. preview):

<ItemGroup>
  <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.8" />
  <PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.8" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.8">
    <PrivateAssets>all</PrivateAssets>
  </PackageReference>
</ItemGroup>


Potem odkomentuj fragment AddAuthentication().AddJwtBearer() w Program.cs.

2) AppDbContext.cs — zamień HasPrecision na HasColumnType

Brakuje Ci rozszerzenia (inne wersje EF lub brak przestrzeni nazw). Najprościej:

using Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.HasDefaultSchema("dbo");

        mb.Entity<Product>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            // ✅ zamiast HasPrecision(18,2):
            b.Property(x => x.Price).HasColumnType("decimal(18,2)");
            b.HasIndex(x => x.Sku).IsUnique(false);
        });

        mb.Entity<User>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Username).HasMaxLength(100).IsRequired();
        });
    }
}

3) ProductEndpoints.cs — popraw regex w trasie (ucieczka nawiasów klamrowych)

Analyzer ASP0017 ma rację: {3,20} w regexie trzeba zduplikować, bo klamry kolidują z parametrem trasy.

Znajdź trasę SKU i zamień:

// było (generowało ostrzeżenia):
// "/sku/{sku:regex(^[A-Z0-9\\-]{3,20}$)}"

// ✅ poprawnie (ucieczka klamerek w regexie):
products.MapGet("/sku/{sku:regex(^[A-Z0-9\\-]{{3,20}}$)}",
    async (string sku, AppDbContext db, CancellationToken ct) =>
{
    var p = await db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Sku == sku, ct);
    return p is null ? Results.NotFound() : Results.Ok(p);
})
.WithName("GetProductBySku")
.Produces<Product>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);


Dodatkowo: jeśli używasz literału zwykłego (nie @), to \\- jest ok; w @-stringu byłoby \ -. Zostaw jak wyżej.

4) (opcjonalnie) Swashbuckle vs wbudowane OpenAPI

Jeśli wolisz pominąć Swashbuckle, w .NET 8/9 możesz użyć wbudowanego pakietu OpenAPI:

builder.Services.AddOpenApi();
...
app.MapOpenApi();
app.UseSwaggerUI(_ => { });


Wtedy nie potrzebujesz Swashbuckle.AspNetCore w csproj.

5) Szybkie komendy po poprawkach
dotnet restore
dotnet clean
dotnet build

# EF (jeśli jeszcze nie masz migracji):
dotnet tool install --global dotnet-ef
dotnet ef migrations add Init --project src/Api/Api.csproj --startup-project src/Api/Api.csproj
dotnet ef database update --project src/Api/Api.csproj --startup-project src/Api/Api.csproj

dotnet watch run --project src/Api/Api.csproj