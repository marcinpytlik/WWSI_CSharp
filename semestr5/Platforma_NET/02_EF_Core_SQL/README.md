# 02 — Od LINQ do SQL

Użyj istniejących demonstracji:
- `demonstracje/08_BibliotekaEf` — model EF Core i SQLite (bez serwera SQL).
- `demonstracje/12_EfCoreSqlLeastPrivilege` — SQL Server, scaffold/migracje, minimalne uprawnienia.
- `demonstracje/58_AsNoTracking` — tracking i brak trackingu.
- `demonstracje/57_EfTransakcja` — transakcja i rollback.

**Przebieg 15 min:** pokaż encję, `DbContext`, zapytanie LINQ, moment materializacji (np. `ToListAsync()`), wydruk SQL (jeśli w projekcie dostępny `ToQueryString()`). Wyjaśnij różnicę między `IQueryable<T>` a `IEnumerable<T>`, lazy loading/N+1 (jeśli włączone). Nie twierdź, że każde LINQ wykonuje SQL — zależy to od providera i źródła.

**Pytania:** kto tłumaczy zapytanie? gdzie wykona się filtr? jak odnaleźć kosztowne zapytanie w SQL Server?
