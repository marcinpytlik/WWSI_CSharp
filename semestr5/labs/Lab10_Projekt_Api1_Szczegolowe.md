# Lab10 Projekt Api1 — szczegółowe laboratorium

**Data:** 2025-10-04  
**Czas:** 120–150 min  
**Stack:** ASP.NET Core 9 (Minimal API), EF Core 9, JWT, xUnit, VS Code  
**Wymagania:** .NET SDK 9.0+, SQL Server 2022 Developer, VS Code, PowerShell 7+, Git

## Cel
- Zbudować od zera usługę **Minimal API** pokrywając temat: Users+Tasks cz.1 (modele, repozytoria).
- Dodać testy integracyjne z `WebApplicationFactory`.
- Przygotować skrypt migracji EF Core i lokalną bazę MSSQL.
- Uporządkować uruchamianie przez `tasks.json` i debug przez `launch.json`.

## Struktura repo
```
/Lab10_Projekt_Api1/
  README.md
  src/Api/
  src/Api/appsettings.json
  src/Api/appsettings.Development.json
  src/Api/Program.cs
  src/Api/Endpoints/*.cs
  src/Api/Domain/*.cs
  src/Api/Infrastructure/*.cs
  tests/Api.Tests/
  .vscode/{
    tasks.json,
    launch.json
  }
```

## Przygotowanie (10 min)
```pwsh
mkdir Lab10_Projekt_Api1; cd Lab10_Projekt_Api1
dotnet new webapi -n Api --use-minimal-apis -f net9.0
dotnet new xunit -n Api.Tests -f net9.0
dotnet sln new Lab10_Projekt_Api1.sln
dotnet sln add src/Api/Api.csproj tests/Api.Tests/Api.Tests.csproj
dotnet add tests/Api.Tests/Api.Tests.csproj reference src/Api/Api.csproj
dotnet add src/Api/Api.csproj package Microsoft.EntityFrameworkCore.SqlServer
dotnet add src/Api/Api.csproj package Microsoft.EntityFrameworkCore.Design
dotnet add src/Api/Api.csproj package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
dotnet add src/Api/Api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add tests/Api.Tests/Api.Tests.csproj package Microsoft.AspNetCore.Mvc.Testing
```

> Jeśli nie używasz `dotnet new webapi`, możesz rozpocząć od `dotnet new web -n Api` i dodać pakiety ręcznie.

## Konfiguracja VS Code
`.vscode/tasks.json`:
```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "type": "process",
      "command": "dotnet",
      "args": ["build"],
      "group": "build",
      "problemMatcher": "$msCompile"
    },
    {
      "label": "test",
      "type": "process",
      "command": "dotnet",
      "args": ["test", "tests/Api.Tests/Api.Tests.csproj", "-l", "trx"],
      "group": "test",
      "problemMatcher": "$msCompile"
    },
    {
      "label": "run:api",
      "type": "process",
      "command": "dotnet",
      "args": ["watch", "run", "--project", "src/Api/Api.csproj"],
      "problemMatcher": "$msCompile",
      "isBackground": true
    }
  ]
}
```

`.vscode/launch.json`:
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Launch API (watch)",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/src/Api/bin/Debug/net9.0/Api.dll",
      "preLaunchTask": "build",
      "cwd": "${workspaceFolder}/src/Api",
      "environment": [ { "name": "ASPNETCORE_ENVIRONMENT", "value": "Development" } ]
    }
  ]
}
```

## Implementacja — kroki
1. **Minimal API & routing**  
   W `Program.cs` zdefiniuj endpointy z `MapGet/MapPost/MapPut/MapDelete`. Dodaj `MapGroup("/api")` i wersjonowanie w ścieżce (`/v1`).

2. **Walidacja (DTO + DataAnnotations / FluentValidation)**  
   - Utwórz katalog `Domain` (encje) i `Endpoints` (handler’y) oraz `Contracts` (DTO).  
   - Przyjmuj DTO w `MapPost` i waliduj: wymagane pola, zakresy. Zwracaj 400 z listą błędów.

3. **EF Core Setup**  
   - Klasa `AppDbContext` (schema `dbo`).  
   - Connection string w `appsettings.Development.json` do lokalnego SQL Server.  
   - `dotnet ef migrations add Init` → `dotnet ef database update`.

4. **CRUD + relacje**  
   - Stwórz encję główną i pomocniczą (np. `Product` ↔ `Category`).  
   - Endpointy: listy, szczegóły, tworzenie, aktualizacja, usunięcie.  
   - `AsNoTracking` na odczytach; `SaveChangesAsync` na mutacjach.

5. **JWT auth**  
   - Dodaj `JwtBearer` z kluczem symetrycznym (sekcja `Jwt` w `appsettings`).  
   - Endpoint `/auth/login` zwracający token dla prostego użytkownika (hard‑coded/in‑memory).  
   - Oznacz wybrane endpointy `[Authorize]` lub `RequireAuthorization()`.

6. **Middleware + logowanie**  
   - Własny middleware rejestrujący czas wykonania i korelację (nagłówek `X-Correlation-Id`).  
   - `UseExceptionHandler` w prod, `UseDeveloperExceptionPage` w dev.

7. **Testy integracyjne**  
   - `WebApplicationFactory<Program>` w projekcie testowym.  
   - Testy: 200 dla GET, 400 dla nieprawidłowego POST, 401 dla endpointu zabezpieczonego bez tokena, 201 dla poprawnego POST.

8. **Obsługa błędów i kontrakty odpowiedzi**  
   - Zwracaj `ProblemDetails` dla błędów.  
   - Dla sukcesów użyj `Results.Ok`, `Results.Created`, `Results.NoContent`.

## Przykładowe fragmenty kodu

`Program.cs` (wycinek):
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Sql")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* TODO: konfiguracja */ });
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseSwagger(); app.UseSwaggerUI();
app.UseAuthentication(); app.UseAuthorization();

var api = app.MapGroup("/api/v1");
api.MapGet("/health", () => Results.Ok(new { status = "ok" }));
// TODO: pozostałe endpointy

app.Run();
```

`Api.Tests/BasicTests.cs`:
```csharp
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class BasicTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public BasicTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/v1/health");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}
```

## Kryteria akceptacji
- [ ] Endpointy zgodne z tematem labu działają i mają testy integracyjne.  
- [ ] Migracje EF odpalają się na lokalnym SQL 2022.  
- [ ] Autoryzacja JWT chroni wybrane ścieżki; nieautoryzowany dostaje 401.  
- [ ] Middleware loguje czas i `X-Correlation-Id`.  
- [ ] README zawiera komendy `dotnet` oraz przykładowe żądania `HTTP`.

## Weryfikacja
- [ ] `dotnet build`, `dotnet test` — zielono.  
- [ ] `curl http://localhost:5xxx/api/v1/health` → 200.  
- [ ] Scenariusze błędne zwracają 4xx z `ProblemDetails`.  

---

> Dla ambitnych: dodać `Serilog`, `OpenAPI` z autoryzacją, seedy danych EF Core, Dockerfile dla API oraz `docker-compose` dla MSSQL.

