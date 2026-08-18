# Demo 60 — gRPC + HTTP API + SQL Server (3 obrazy)

Kontrakt w `proto/notes.proto`. Serwer gRPC trzyma notatki w SQL.
HTTP API to cienki adapter (`INotesClient`). Testy **nie** wołają Dockera:
SQLite + fake klienta HTTP.

| Kontener | Obraz | Port | Rola |
|---|---|---|---|
| `sqlserver` | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 | notatki |
| `grpc` | `demo60-grpc:local` | (wewnętrzny 8080) | `Notes` service |
| `api` | `demo60-api:local` | 5361 | REST → gRPC |

```bash
cd demonstracje/60_GrpcSql && docker compose up --build
curl -X POST http://localhost:5361/api/v1/notes -H "Content-Type: application/json" -d '{"title":"Przez gRPC"}'
```

Hasło `sa` / `Demo60_StrongPass!`. Port 1433 może kolidować z innymi demami.
