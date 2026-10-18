# Diagnostyka

PID, dotnet-counters, GC, liczniki, logi; użyj działającej aplikacji ShowLab.

## Uruchomienie (PowerShell z korzenia repo)
```powershell
dotnet run --project DotNet_ShowLab/02_Memory_GC/src/Demo.csproj -- retained
# w drugim terminalu
dotnet-counters ps
dotnet-counters monitor -p <PID> System.Runtime
```

## Przebieg
1. Zapytaj studentów, czego się spodziewają.
2. Uruchom pokaz i porównaj pomiary lub odpowiedzi.
3. Omów wynik oraz ograniczenia eksperymentu.

## Interpretacja
PID, dotnet-counters, GC, liczniki, logi; użyj działającej aplikacji ShowLab.


