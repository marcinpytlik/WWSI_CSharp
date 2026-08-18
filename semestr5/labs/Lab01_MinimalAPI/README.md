# Lab01 Minimal API — referencyjna implementacja (.NET 10)

```bash
cd semestr5/labs/Lab01_MinimalAPI
dotnet test
dotnet run --project src/Api --urls http://localhost:5080
```

- `GET /health` → `{ "status": "ok", "utc": "..." }`
- `GET /hello` → `{ "message": "Hello, world" }`
- `GET /hello?name=Ada` → `{ "message": "Hello, Ada" }`

Bez bazy i bez JWT — to celowo Lab01, nie szablon semestru.
