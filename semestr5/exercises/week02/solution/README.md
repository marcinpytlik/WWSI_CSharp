# Semestr 5 — Tydzień 2: rozwiązania wzorcowe

Projekt .NET 10 Minimal API realizuje pięć [ćwiczeń tygodnia 2](../README.md) (100 pkt).

```powershell
cd semestr5/exercises/week02/solution
dotnet build Week02.Solutions.slnx
dotnet run --project Week02.Api --urls http://localhost:5102
```

| Ćwiczenie | Punkty | Endpoint/przykład |
|---|---:|---|
| 1. Route parameter | 10 | `GET /api/v1/users/{id:int}` — 200/400/404 (np. id 1, 0, 999) |
| 2. Query parameters | 15 | `GET /api/v1/users?search=Anna&page=1&pageSize=10` |
| 3. Route groups | 20 | `/api/v1/users` i `/api/v1/tasks` |
| 4. Typed results | 25 | `Results<Ok<T>,BadRequest<string>,NotFound<string>>` |
| 5. Wersjonowanie DTO | 30 | `GET /api/v1/products` i `GET /api/v2/products` |

```powershell
Invoke-RestMethod http://localhost:5102/api/v1/users/1
Invoke-WebRequest -SkipHttpErrorCheck http://localhost:5102/api/v1/users/0
Invoke-WebRequest -SkipHttpErrorCheck http://localhost:5102/api/v1/users/999
Invoke-RestMethod 'http://localhost:5102/api/v1/users?search=Anna&page=1&pageSize=10'
Invoke-RestMethod http://localhost:5102/api/v1/tasks
Invoke-RestMethod http://localhost:5102/api/v1/products
Invoke-RestMethod http://localhost:5102/api/v2/products
```

**Ważne:** ograniczenie trasy `{id:int}` oznacza, że wejście np. `/users/abc` zwraca **404 na poziomie routingu**, nie 400; wartość `0` jest poprawną liczbą całkowitą, ale zwraca **400 z walidacji**. Dane są przykładowe i przechowywane w pamięci. DTO v1 zachowuje kontrakt, v2 dodaje walutę i kategorię bez duplikowania modelu domenowego.
