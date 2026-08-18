# Demo 59 — OpenTelemetry + SQL Server + Jaeger (3 obrazy)

API zapisuje zdarzenie w SQL i emituje span `create-event` (`ActivitySource`).
W Dockerze spany idą OTLP do **Jaeger**. Testy używają `ActivityListener` — bez Jaegera.

| Kontener | Obraz | Port | Rola |
|---|---|---|---|
| `sqlserver` | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 | zdarzenia |
| `jaeger` | `jaegertracing/all-in-one` | 16686 / 4317 | UI + OTLP |
| `api` | `demo59-api:local` | 5359 | Minimal API |

```bash
cd demonstracje/59_OpenTelemetrySql && docker compose up --build
curl -X POST http://localhost:5359/api/v1/events -H "Content-Type: application/json" -d '{"name":"Sala OTEL"}'
```

UI: http://localhost:16686 — serwis `demo59-api`.
Hasło `sa` / `Demo59_StrongPass!`. Port 1433 może kolidować z innymi demami.
