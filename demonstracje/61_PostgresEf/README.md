# Demo 61 — EF Core + PostgreSQL (3 obrazy)

Ten sam CRUD co demo 08, ale `UseNpgsql` zamiast SQL Server / SQLite.
W testach `Testing=true` + SQLite — pokazuje, że model EF nie jest przyklejony do silnika.

| Kontener | Obraz | Port | Rola |
|---|---|---|---|
| `postgres` | `postgres:16-alpine` | 5432 | baza |
| `api` | `demo61-api:local` | 5362 | Minimal API |
| `adminer` | `adminer` | 5363 | UI SQL |

```bash
cd demonstracje/61_PostgresEf && docker compose up --build
curl -X POST http://localhost:5362/api/v1/books -H "Content-Type: application/json" -d '{"title":"Clean Code","year":2008}'
```

Adminer: http://localhost:5363 (system PostgreSQL, serwer `postgres`, user `demo61`).
Hasło `Demo61_StrongPass!` tylko do compose.
