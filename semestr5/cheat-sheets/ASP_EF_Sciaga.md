# Ściąga — Minimal API, EF Core, JWT (semestr 5)

## Minimal API (.NET 10)

```csharp
var app = WebApplication.CreateBuilder(args).Build();
var v1 = app.MapGroup("/api/v1");
v1.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Run();
public partial class Program { }
```

Kody: 200 OK, 201 Created, 204 No Content, 400 walidacja, 401 brak tokena, 403 brak prawa, 404 brak zasobu.

## EF Core

- `DbContext` + `DbSet<T>`
- laby: **SQLite**; SQL Server opcjonalnie
- relacje: Fluent `HasMany` / `WithOne`
- na zewnątrz **DTO** (`Select`), nie cykle nawigacji
- `SaveChangesAsync`

## JWT

- `AddAuthentication().AddJwtBearer`
- `.RequireAuthorization()`
- sekret: user-secrets / env, min. długość klucza
- hasła: `PasswordHasher<TUser>`

## Testy

`WebApplicationFactory<Program>` + izolowana baza. Zero `Thread.Sleep`.

## Bezpieczeństwo na sali

- nie loguj `Authorization` ani haseł
- nie commituj `*.db` ani wypełnionych dzienników ocen
