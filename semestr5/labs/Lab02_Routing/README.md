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
