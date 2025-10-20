# Lab01 MinimalAPI — szczegółowe laboratorium

**Data:** 2025-10-04  
**Czas:** 120–150 min  
**Stack:** ASP.NET Core 9 (Minimal API), EF Core 9, JWT, xUnit, VS Code  
**Wymagania:** .NET SDK 9.0+, SQL Server 2022 Developer, VS Code, PowerShell 7+, Git

## Cel
- Minimal API (`MapGet`, health, grupowanie tras `/api/v1`), konfiguracja Kestrel (domyślna).
- Testy integracyjne (`WebApplicationFactory`).
- EF Core: `AppDbContext`, migracje, SQL Server lokalnie.
- JWT auth dla wybranych endpointów.
- VS Code: `tasks.json`, `launch.json`.

## Struktura repo
```
/Lab01_MinimalAPI/
  README.md
  src/Api/
    Api.csproj
    Program.cs
    appsettings.json
    appsettings.Development.json
    Domain/
    Infrastructure/
    Endpoints/
    Contracts/
  tests/Api.Tests/
    Api.Tests.csproj
    BasicTests.cs
  .vscode/
    tasks.json
    launch.json
```

## Przygotowanie (komendy)
```pwsh
cd Lab01_MinimalAPI
dotnet restore
dotnet build
dotnet test
dotnet watch run --project src/Api/Api.csproj
```

## EF Core (migracje)
> Upewnij się, że connection string wskazuje na lokalny SQL Server 2022.

```pwsh
dotnet tool install --global dotnet-ef
dotnet ef migrations add Init --project src/Api/Api.csproj --startup-project src/Api/Api.csproj
dotnet ef database update --project src/Api/Api.csproj --startup-project src/Api/Api.csproj
```

## Endpoints (przykłady)
- `GET /api/v1/health` → 200
- `POST /api/v1/auth/login` → 200 `{ token: "..." }` dla `admin` / `Password!123`
- `GET /api/v1/secure/ping` → 401 bez tokena
- `GET /api/v1/products` → lista
- `POST /api/v1/products` → walidacja, 201

## Testy
```pwsh
dotnet test tests/Api.Tests/Api.Tests.csproj
```

## Debug w VS Code
- Task: `run:api`
- Debug: `Launch API (watch)`

---

> Dla ambitnych: dodaj Serilog, seedy EF, Dockerfile + docker-compose (mssql).
