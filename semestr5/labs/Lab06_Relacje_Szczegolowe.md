# Lab06 Relacje — szczegółowe laboratorium

**Czas:** 120 min · EF Core

## Cel

Model **User 1 — n TaskItem**. Uniknąć cykli JSON.

## Zadania

1. `User { Id, Email, Tasks }`, `TaskItem { Id, Title, UserId, User }`.
2. Fluent API: `HasMany(u => u.Tasks).WithOne(t => t.User).HasForeignKey(t => t.UserId)`.
3. `GET /api/v1/users/{id}/tasks` z `Include` **albo** lepiej projekcją `Select` do DTO.
4. Nie serializuj `User.Tasks.User.Tasks...`.

## Kryteria

- [ ] FK w bazie (możesz sprawdzić w SQLite)
- [ ] endpoint zwraca płaskie DTO
- [ ] user bez zadań → pusta lista 200, nie 404
