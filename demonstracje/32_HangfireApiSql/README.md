# Demo 32 — API + SQL Server + Hangfire (3 obrazy Docker)

Proste API przyjmuje raport, wrzuca zadanie do **Hangfire** (SQL Server jako storage).
Osobny kontener **workera** wykonuje job i zapisuje wynik w tej samej bazie.

| Kontener | Obraz | Port | Rola |
|---|---|---|---|
| `sqlserver` | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 | dane + tabele Hangfire |
| `api` | `demo32-api:local` (budowany) | 5080 | Minimal API, tylko **enqueue** (bez serwera Hangfire) |
| `hangfire` | `demo32-hangfire:local` (budowany) | 5081 | worker + dashboard `/hangfire` |

Testy jednostkowe **nie** wymagają Dockera.

## Start na sali

```bash
cd demonstracje/32_HangfireApiSql
docker compose up --build
```

Poczekaj, aż SQL wstanie (worker ponawia połączenie).

```bash
curl -X POST http://localhost:5080/api/v1/reports \
  -H "Content-Type: application/json" \
  -d '{"title":"Sales Q1"}'

curl http://localhost:5080/api/v1/reports
```

Dashboard: http://localhost:5081/hangfire (w demo bez logowania — nie na produkcję).

Hasło `sa` / `Demo32_StrongPass!` wyłącznie do lokalnego compose. Jeśli 1433 jest zajęte (demo 11/12), zatrzymaj tamten compose.

## Co pokazać studentom

1. **Trzy obrazy** — `docker compose images` / `docker compose ps`.
2. API nie przetwarza joba: `AddHangfire` bez `AddHangfireServer`.
3. Worker ma `AddHangfireServer` i dashboard.
4. Po POST status `Queued` → po chwili `Done` (GET `/api/v1/reports`).
5. Dashboard: succeeded jobs.

## Testy bez Dockera

```bash
dotnet test demonstracje/32_HangfireApiSql/tests/Demo32_HangfireApiSql.Tests.csproj
```
