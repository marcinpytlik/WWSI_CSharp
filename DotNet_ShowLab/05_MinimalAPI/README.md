# Minimal API

Uruchom mały serwer HTTP, wykonaj GET i POST; zobacz JSON i routing.

## Uruchomienie (PowerShell z korzenia repo)
```powershell
dotnet run --project DotNet_ShowLab/05_MinimalAPI/src/Demo.csproj --urls http://localhost:5088
# w drugim terminalu:
Invoke-RestMethod http://localhost:5088/hello/Marcin
Invoke-RestMethod http://localhost:5088/echo -Method Post -ContentType application/json -Body '{"text":"test"}'
```

## Przebieg
1. Zapytaj studentów, czego się spodziewają.
2. Uruchom pokaz i porównaj pomiary lub odpowiedzi.
3. Omów wynik oraz ograniczenia eksperymentu.

## Interpretacja
Uruchom mały serwer HTTP, wykonaj GET i POST; zobacz JSON i routing.


