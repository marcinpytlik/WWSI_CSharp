# Demo 40 — API + MinIO + SQL Server (3 obrazy Docker)

Upload pliku: bajty do **MinIO** (S3), metadane do **SQL Server**.
W testach `IBlobStore` to `InMemoryBlobStore` + SQLite — bez Dockera.

| Kontener | Obraz | Port | Rola |
|---|---|---|---|
| `sqlserver` | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 | metadane plików |
| `minio` | `minio/minio` | 9000 / 9001 | obiekty (konsola 9001) |
| `api` | `demo40-api:local` (budowany) | 5340 | Minimal API |

## Start na sali

```bash
cd demonstracje/40_MinioUploadSql
docker compose up --build
```

```bash
curl -F "file=@README.md" http://localhost:5340/api/v1/files
curl http://localhost:5340/api/v1/files
```

Konsola MinIO: http://localhost:9001 (`demo40access` / `demo40secret1` — tylko demo).
Hasło SQL `sa` / `Demo40_StrongPass!`. Port 1433 może kolidować z innymi demami.

## Co pokazać studentom

1. Trzy obrazy.
2. Kontrakt `IBlobStore` — ten sam endpoint, inna implementacja w testach.
3. SQL trzyma nazwę i klucz, nie zawartość pliku.
4. Konsola MinIO pokazuje bucket `demo40`.

## Testy bez Dockera

```bash
dotnet test demonstracje/40_MinioUploadSql/tests/Demo40_MinioUploadSql.Tests.csproj
```
