# Pamięć i GC

Tryby: transient, retained, LOH. Obserwuj GC.CollectionCount i dotnet-counters; alokacje są ograniczone.

## Uruchomienie (PowerShell z korzenia repo)
```powershell
dotnet run --project DotNet_ShowLab/02_Memory_GC/src/Demo.csproj -c Release -- transient
dotnet run --project DotNet_ShowLab/02_Memory_GC/src/Demo.csproj -c Release -- retained
dotnet run --project DotNet_ShowLab/02_Memory_GC/src/Demo.csproj -c Release -- loh
```

## Przebieg
1. Zapytaj studentów, czego się spodziewają.
2. Uruchom pokaz i porównaj pomiary lub odpowiedzi.
3. Omów wynik oraz ograniczenia eksperymentu.

## Interpretacja
Tryby: transient, retained, LOH. Obserwuj GC.CollectionCount i dotnet-counters; alokacje są ograniczone.


