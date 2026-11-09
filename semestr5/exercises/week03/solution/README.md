# Semestr 5 — Tydzień 3: rozwiązania wzorcowe

Projekt **.NET 10 Minimal API** realizuje pięć [ćwiczeń tygodnia 3](../README.md) (100 pkt).

## Uruchomienie

```powershell
cd semestr5/exercises/week03/solution
dotnet build Week03.Solutions.slnx
dotnet run --project Week03.Api --urls http://localhost:5103
```

## Ćwiczenia

| Zadanie | Punkty | Implementacja |
|---|---:|---|
| 1. DTO wejściowe | 10 | `CreateUserRequest` osobno od `User`, mapowanie do `UserResponse`; klient nie podaje Id |
| 2. DataAnnotations | 15 | `Required`, `StringLength`, `EmailAddress`, `Range`; własne komunikaty |
| 3. Reguła biznesowa | 20 | Unikalność loginu bez względu na wielkość liter; osobny `UserCatalog`; konflikt 409 |
| 4. ProblemDetails | 25 | `Results.ValidationProblem` z błędami pól i HTTP 400 |
| 5. Pipeline walidacji | 30 | Jeden `ValidationFilter<TRequest>` współdzielony przez POST i PUT |

## Scenariusze weryfikacyjne (PowerShell 7)

```powershell
$base = 'http://localhost:5103/api/v1/users'
$headers = @{ 'Content-Type' = 'application/json' }

$good = '{"login":"student01","email":"student@example.com","age":22}'
$bad = '{"login":"ab","email":"zly-email","age":12}'
$another = '{"login":"student02","email":"student2@example.com","age":24}'

# Poprawny POST → 201 i Location
Invoke-WebRequest -Uri $base -Method Post -Headers $headers -Body $good

# Niepoprawny POST → 400 (ValidationProblemDetails z polami Login, Email, Age)
$invalid = Invoke-WebRequest -SkipHttpErrorCheck -Uri $base -Method Post -Headers $headers -Body $bad
$invalid.StatusCode
$invalid.Content | ConvertFrom-Json | Format-List

# Powtórzony login w innej wielkości liter → 409 (ProblemDetails)
$duplicate = '{"login":"STUDENT01","email":"other@example.com","age":25}'
(Invoke-WebRequest -SkipHttpErrorCheck -Uri $base -Method Post -Headers $headers -Body $duplicate).StatusCode

# Drugi poprawny użytkownik → 201
Invoke-WebRequest -Uri $base -Method Post -Headers $headers -Body $another

# PUT używa dokładnie tego samego filtra — błędny → 400
(Invoke-WebRequest -SkipHttpErrorCheck -Uri "$base/1" -Method Put -Headers $headers -Body $bad).StatusCode

# Zajęty login przy aktualizacji → 409
(Invoke-WebRequest -SkipHttpErrorCheck -Uri "$base/2" -Method Put -Headers $headers -Body $good).StatusCode

# Poprawny PUT → 204; GET → 200
$changed = '{"login":"student03","email":"new@example.com","age":23}'
(Invoke-WebRequest -Uri "$base/1" -Method Put -Headers $headers -Body $changed).StatusCode
Invoke-RestMethod "$base/1"
```

**Uwagi dydaktyczne:** Dane są przechowywane wyłącznie w pamięci (restart kasuje użytkowników). W praktyce reguła unikalności wymaga także indeksu unikalnego w bazie. Filtr waliduje adnotacje właściwości DTO, a katalog odpowiada za walidację biznesową. Przy niepoprawnym JSON lub braku body mechanizm wiązania żądania w ASP.NET Core może odrzucić żądanie jeszcze przed filtrem. To przykład edukacyjny, a nie implementacja produkcyjnego magazynu użytkowników.
