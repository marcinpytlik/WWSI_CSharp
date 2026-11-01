# Semestr 5 — Tydzień 1: rozwiązania wzorcowe

Projekt .NET 10 Minimal API realizuje 5 zadań z [treści ćwiczeń](../README.md), łącznie 100 pkt.

```powershell
cd semestr5/exercises/week01/solution
dotnet build Week01.Solutions.slnx
dotnet run --project Week01.Api --urls http://localhost:5101
```

Przykłady: `GET /api/v1/hello` (10 pkt), `GET /api/v1/health` (15 pkt), `GET /api/v1/config` (20 pkt), `MapGroup("/api/v1")` (25 pkt), `GET /api/v1/notes`, `GET /api/v1/notes/{id}`, `POST /api/v1/notes`, `DELETE /api/v1/notes/{id}` (30 pkt).

```powershell
Invoke-RestMethod http://localhost:5101/api/v1/hello
Invoke-RestMethod http://localhost:5101/api/v1/health
Invoke-RestMethod http://localhost:5101/api/v1/config
Invoke-RestMethod -Method Post -Uri http://localhost:5101/api/v1/notes -ContentType application/json -Body '{"text":"Pierwsza notatka"}'
Invoke-RestMethod http://localhost:5101/api/v1/notes
```

Wspólnie omówcie: statusy 200/201/404/204, walidację 400, DTO, Options i brak sekretów w odpowiedzi. To pamięć procesu: po restarcie notatki znikają. Dla dydaktycznej prostoty API nie ma uwierzytelniania i nie jest gotowe na produkcję.
