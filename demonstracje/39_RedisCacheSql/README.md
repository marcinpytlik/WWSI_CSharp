# Demo 39 — API + Redis + SQL Server (3 obrazy Docker)

Lista produktów idzie przez `IDistributedCache`. W Dockerze cache to **Redis**,
w testach — `AddDistributedMemoryCache` + SQLite. Po POST klucz cache jest usuwany.

| Kontener | Obraz | Port | Rola |
|---|---|---|---|
| `sqlserver` | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 | produkty |
| `redis` | `redis:7-alpine` | 6379 | cache |
| `api` | `demo39-api:local` (budowany) | 5339 | Minimal API |

Testy **nie** wymagają Dockera.

## Start na sali

```bash
cd demonstracje/39_RedisCacheSql
docker compose up --build
```

```bash
curl -X POST http://localhost:5339/api/v1/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Notes"}'

curl http://localhost:5339/api/v1/products
```

Hasło `sa` / `Demo39_StrongPass!` wyłącznie do lokalnego compose. Port 1433 może kolidować z demo 11/12/32.

## Co pokazać studentom

1. Trzy obrazy: `docker compose ps`.
2. Ten sam kod (`IDistributedCache`) — Redis na sali, pamięć w testach.
3. Drugi GET nie musi iść do SQL (w Redis CLI: `KEYS *` / `GET products:all`).
4. POST czyści cache — nowy produkt od razu widać na liście.

## Testy bez Dockera

```bash
dotnet test demonstracje/39_RedisCacheSql/tests/Demo39_RedisCacheSql.Tests.csproj
```
