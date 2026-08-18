# Lab10 Projekt Users+Tasks (część 1)

**Czas:** 120 min

## Cel

Szkielet zaliczeniowego API **bez JWT** (auth w Lab11).

## Zakres

- encje User, TaskItem + relacja 1–n
- CRUD Users
- CRUD Tasks **z** `userId` w ścieżce `/api/v1/users/{userId}/tasks`
- SQLite, migracja albo EnsureCreated (Development)
- README z `dotnet run` i przykładowym `curl`

## Kryteria części 1

- [ ] kompiluje się
- [ ] da się utworzyć usera i jego taska
- [ ] 404 gdy user nie istnieje
