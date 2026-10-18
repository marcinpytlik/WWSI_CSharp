# Asynchroniczność

Porównaj sekwencyjne Task.Delay z Task.WhenAll; potem CPU-bound z Parallel.For. Async nie oznacza nowego wątku.

## Uruchomienie (PowerShell z korzenia repo)
```powershell
dotnet run --project DotNet_ShowLab/03_Async_Parallel/src/Demo.csproj -c Release
```

## Przebieg
1. Zapytaj studentów, czego się spodziewają.
2. Uruchom pokaz i porównaj pomiary lub odpowiedzi.
3. Omów wynik oraz ograniczenia eksperymentu.

## Interpretacja
Porównaj sekwencyjne Task.Delay z Task.WhenAll; potem CPU-bound z Parallel.For. Async nie oznacza nowego wątku.


