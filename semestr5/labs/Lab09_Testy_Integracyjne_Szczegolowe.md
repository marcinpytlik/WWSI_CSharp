# Lab09 Testy integracyjne — szczegółowe laboratorium

**Czas:** 120 min · `Microsoft.AspNetCore.Mvc.Testing`

## Cel

Testować API jak klient HTTP, z izolowaną bazą.

## Zadania

1. `WebApplicationFactory<Program>` + `ConfigureWebHost` podmieniający `DbContext` na SQLite `Filename=:memory:` albo EF InMemory.
2. Testy: POST note → GET by id; GET nieistniejący → 404; walidacja → 400.
3. Nie używaj `Thread.Sleep`.
4. Każdy test dostaje czystą bazę (nowy scope / nowa factory).

## Kryteria

- [ ] min. 4 testy integracyjne
- [ ] zero zależności od kolejności testów
