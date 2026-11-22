# Semestr 5 — Tydzień 5: przykładowe rozwiązanie CRUD z EF Core

Samodzielny projekt dydaktyczny **.NET 10 Minimal API + EF Core 10 + SQLite** realizuje [pięć ćwiczeń](../README.md), razem **100 pkt**. Oddzielne DTO zapobiegają overpostingowi. SQLite pozwala uruchomić demonstrację bez zewnętrznego SQL Server.

## Uruchomienie (PowerShell 7)

~~~powershell
cd semestr5/exercises/week05/solution
dotnet restore Week05.Solutions.slnx
dotnet build Week05.Solutions.slnx
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project Week05.Api --urls http://localhost:5105
~~~

W Development tworzona jest lokalna baza SQLite (plik względem katalogu roboczego). **Uwaga:** to niezależna demonstracja CRUD, używa EnsureCreatedAsync, **nie** migracji. W projekcie kontynuującym tydzień 4 należy stosować migracje EF Core (MigrateAsync) i nie łączyć EnsureCreated z migracjami. Poza Development aplikacja nie tworzy automatycznie schematu.

## Zadania

| Zadanie | Punkty | Implementacja |
|---|---:|---|
| 1. Create | 10 | POST /users, SaveChangesAsync, 201 Created + Location |
| 2. Read | 15 | GET /users, GET /users/{id}, AsNoTracking, 404 |
| 3. Update | 20 | PUT /users/{id}, walidacja, 200/404 |
| 4. Delete | 25 | DELETE /users/{id}, Remove, SaveChangesAsync, 204/404 |
| 5. CRUD produkcyjny | 30 | DTO, stronicowanie (page, pageSize), search, sortBy, descending, metadane TotalCount/TotalPages, async |

## Przykładowy test PowerShell

~~~powershell
$base = "http://localhost:5105/users"
$new = Invoke-RestMethod -Method Post -Uri $base -ContentType "application/json" -Body '{"name":"Anna Nowak","email":"anna@example.test"}'
$new
Invoke-RestMethod "$base?page=1&pageSize=5&search=Anna&sortBy=name"
Invoke-RestMethod "$base/$($new.id)"
Invoke-RestMethod -Method Put -Uri "$base/$($new.id)" -ContentType "application/json" -Body '{"name":"Anna Kowalska","email":"anna@example.test"}'
Invoke-RestMethod -Method Delete -Uri "$base/$($new.id)"
(Invoke-WebRequest -SkipHttpErrorCheck "$base/$($new.id)").StatusCode # 404
~~~

Przetestuj również: powtórny adres e-mail (409), niepoprawny email (400), pageSize=101 (400), nieznany sortBy (400), usuwanie brakującego rekordu (404), oraz nagłówek Location odpowiedzi POST.

**Uwagi dydaktyczne:** Aplikacja pokazuje podstawowy CRUD, a nie kompletny system produkcyjny. Dalsze etapy to migracje, obsługa konfliktów współbieżności, uwierzytelnianie, autoryzacja, testy integracyjne i bezpieczny sposób wdrożeń. Unikalny indeks e-mail jest warstwą zabezpieczenia bazy; współbieżne POST nadal wymagają obsługi wyjątków naruszenia ograniczeń.
