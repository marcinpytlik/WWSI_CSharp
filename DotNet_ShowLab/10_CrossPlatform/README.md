# Windows i Linux

Publikacja aplikacji i uruchomienie jej w obrazie dotnet/runtime:10.0 na Docker Linux.

## Uruchomienie (PowerShell z korzenia repo)
```powershell
dotnet run --project DotNet_ShowLab/03_Async_Parallel/src/Demo.csproj
docker build -f DotNet_ShowLab/10_CrossPlatform/Dockerfile -t wwsi-showlab .
docker run --rm wwsi-showlab
```

## Przebieg
1. Zapytaj studentów, czego się spodziewają.
2. Uruchom pokaz i porównaj pomiary lub odpowiedzi.
3. Omów wynik oraz ograniczenia eksperymentu.

## Interpretacja
Publikacja aplikacji i uruchomienie jej w obrazie dotnet/runtime:10.0 na Docker Linux.


