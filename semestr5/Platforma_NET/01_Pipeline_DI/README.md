# 01 — ASP.NET Core: pipeline middleware i DI

Z głównego katalogu repo:
```powershell
dotnet run --project semestr5/Platforma_NET/01_Pipeline_DI/src/PipelineDemo.csproj --urls http://localhost:5000
```
Drugi terminal:
```powershell
$base = 'http://localhost:5000'
Invoke-RestMethod "$base/health"
Invoke-RestMethod "$base/orders/42"
Invoke-RestMethod "$base/orders/42"
try { Invoke-RestMethod "$base/fail" } catch { $_.Exception.Message }
```

**Obserwuj:** każde żądanie otrzymuje nowy `X-Correlation-ID`; `RequestStamp` jest `scoped` (jeden identyfikator w obrębie requestu); middleware loguje początek, koniec i czas; `/fail` kończy się 500 bez ujawniania szczegółów wyjątku klientowi. Kod specjalnie loguje tę samą wartość identyfikatora w middleware i endpointzie.

**Pytania:** co zmieni zmiana kolejności middleware? co stanie się bez `await next()`? czym różni się singleton od scoped?

Zatrzymaj proces Ctrl+C.
