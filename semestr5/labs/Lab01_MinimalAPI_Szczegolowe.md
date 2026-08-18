# Lab01 Minimal API — szczegółowe laboratorium

**Czas:** 90–120 min · **Stack:** ASP.NET Core 10 (Minimal API), xUnit  
**Wymagania:** .NET SDK 10.x — **bez** SQL Server i **bez** JWT na tym labie.

## Cel

Postawić najcieńsze możliwe HTTP API: health + hello. W repo jest referencja:
`semestr5/labs/Lab01_MinimalAPI/`.

```bash
cd semestr5/labs/Lab01_MinimalAPI
dotnet test
dotnet run --project src/Api --urls http://localhost:5080
curl -s http://localhost:5080/health
```

## Zadania

1. `GET /health` → `{ "status": "ok", "utc": "..." }` status 200.
2. `GET /hello?name=Ada` → `{ "message": "Hello, Ada" }`; brak name → `Hello, world`.
3. Nie dodawaj EF Core ani JWT.
4. Test integracyjny `WebApplicationFactory`: health 200 i JSON zawiera `status`.

## Kryteria

- [ ] dwa endpointy, zero bazy
- [ ] testy nie wymagają otwartego portu (factory)
- [ ] `Program` ma `public partial class Program;` pod testy
