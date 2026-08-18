# Lab04 EF Core setup — szczegółowe laboratorium

**Czas:** 120 min · EF Core 10 + **SQLite** (domyślnie)

SQL Server nie jest wymagany na sali. Jeśli masz LocalDB — wariant opcjonalny.

## Cel

Podłączyć `DbContext` i utworzyć schemat.

## Zadania

1. `Note { Id, Title, Body, CreatedUtc }`.
2. `AppDbContext : DbContext` + `UseSqlite("Data Source=lab04.db")`.
3. Rejestracja: `builder.Services.AddDbContext<AppDbContext>(...)`.
4. Development: `db.Database.EnsureCreated()` **albo** migracja `dotnet ef migrations add Init`.
5. Nie commituj `*.db`.

## Kryteria

- [ ] aplikacja startuje bez ręcznego klikania w SSMS
- [ ] connection string w `appsettings.Development.json`, nie w kodzie na sztywno (poza testami)
- [ ] brak JWT na tym labie
