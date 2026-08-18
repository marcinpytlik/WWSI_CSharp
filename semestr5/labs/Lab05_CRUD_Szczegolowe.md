# Lab05 CRUD — szczegółowe laboratorium

**Czas:** 120 min · Minimal API + EF Core (SQLite)

## Cel

Pełny CRUD notatek z poprawnymi kodami HTTP.

## Endpointy

| Metoda | Ścieżka | Kod sukcesu |
|---|---|---|
| GET | `/api/v1/notes` | 200 lista DTO |
| GET | `/api/v1/notes/{id:guid}` | 200 / 404 |
| POST | `/api/v1/notes` | 201 + Location |
| PUT | `/api/v1/notes/{id:guid}` | 204 / 404 |
| DELETE | `/api/v1/notes/{id:guid}` | 204 / 404 |

Wszystko `async`. Nie zwracaj encji śledzonej bez DTO, jeśli masz relacje (tu jeszcze nie).

## Kryteria

- [ ] 5 endpointów + testy przynajmniej GET lista i POST
- [ ] `SaveChangesAsync`
