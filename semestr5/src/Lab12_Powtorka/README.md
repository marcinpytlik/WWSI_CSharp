# Lab12 Powtorka — szablon projektu

## Komendy
```pwsh
dotnet build
dotnet test tests/Api.Tests/Api.Tests.csproj
dotnet watch run --project src/Api/Api.csproj
```
- Swagger: `https://localhost:5001/swagger`
- Health: `GET /api/v1/health`
- CRUD: `GET/POST/PUT/DELETE /api/v1/products`

## EF Core
- Connection string: `appsettings.Development.json` → `(localdb)\mssqllocaldb` (zmień na swój SQL 2022).

## JWT
- Klucz w `appsettings.json` → `Jwt:Key` (zmień na własny).

## Testy integracyjne
- Oparte o `WebApplicationFactory<Program>`.
