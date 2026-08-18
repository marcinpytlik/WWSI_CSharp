# Demo 41 — YARP gateway + API + SQL Server (3 obrazy Docker)

Studenci wołają **bramkę** (`/api/...`), YARP przekazuje do API. SQL jest tylko za API.

| Kontener | Obraz | Port | Rola |
|---|---|---|---|
| `sqlserver` | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 | notatki |
| `api` | `demo41-api:local` (budowany) | (wewnętrzny 8080) | Minimal API |
| `gateway` | `demo41-gateway:local` (budowany) | 5341 | YARP reverse proxy |

Testy **nie** wymagają Dockera (SQLite + `Testing=true` na bramce bez klastrów).

## Start na sali

```bash
cd demonstracje/41_YarpGateway
docker compose up --build
```

```bash
curl http://localhost:5341/health
curl -X POST http://localhost:5341/api/v1/notes \
  -H "Content-Type: application/json" \
  -d '{"title":"Przez YARP"}'
curl http://localhost:5341/api/v1/notes
```

Hasło `sa` / `Demo41_StrongPass!`. Port 1433 może kolidować z innymi demami.

## Co pokazać studentom

1. Trzy obrazy — API nie jest wystawione na hosta, tylko gateway.
2. Konfiguracja tras w `src/Gateway/appsettings.json`.
3. `/health` na bramce nie idzie do SQL.

## Testy bez Dockera

```bash
dotnet test demonstracje/41_YarpGateway/tests/Demo41_YarpGateway.Tests.csproj
```
