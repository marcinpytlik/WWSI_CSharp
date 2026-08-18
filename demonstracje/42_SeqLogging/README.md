# Demo 42 — API + Seq + SQL Server (3 obrazy Docker)

Serilog pisze na konsolę i (w Dockerze) do **Seq**. Każdy request ma `X-Correlation-Id`.
Testy nie startują Seq — sink jest wyłączany przez `Testing=true`.

| Kontener | Obraz | Port | Rola |
|---|---|---|---|
| `sqlserver` | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 | zdarzenia |
| `seq` | `datalust/seq` | 5344 | UI logów |
| `api` | `demo42-api:local` (budowany) | 5343 | Minimal API |

## Start na sali

```bash
cd demonstracje/42_SeqLogging
docker compose up --build
```

```bash
curl -X POST http://localhost:5343/api/v1/events \
  -H "Content-Type: application/json" \
  -H "X-Correlation-Id: sala-1" \
  -d '{"message":"Hello Seq"}'
```

UI Seq: http://localhost:5344 — wyszukaj `sala-1`.

Hasło SQL `sa` / `Demo42_StrongPass!`. Port 1433 może kolidować z innymi demami.

## Co pokazać studentom

1. Trzy obrazy.
2. Correlation id w nagłówku odpowiedzi i w Seq.
3. Testy CI bez Seq.

## Testy bez Dockera

```bash
dotnet test demonstracje/42_SeqLogging/tests/Demo42_SeqLogging.Tests.csproj
```
