# Lab02 Routing — szczegółowe laboratorium

**Czas:** 90–120 min · ASP.NET Core 10 · **bez EF / JWT**

## Cel

Zorganizować endpointy przez `MapGroup` i constraint'y.

## Zadania

1. Prefiks `/api/v1`.
2. `GET /api/v1/notes/{id:int}` — `id < 1` → 400; nieistniejący → 404. Słownik w pamięci.
3. `GET /api/v1/notes?tag=work` — filtr opcjonalny.
4. Nie używaj jednego `MapGet("/{everything}")`.

## Testy

- 200 gdy id istnieje
- 404 gdy nie
- 400 gdy id=0 (constraint albo walidacja)

## Kryteria

- [ ] `MapGroup("/api/v1")`
- [ ] spójne nazwy endpointów `.WithName(...)`
