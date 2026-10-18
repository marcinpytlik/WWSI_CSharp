# Wyścig danych

Równoległa inkrementacja bez synchronizacji kontra Interlocked. Wynik błędny nie jest gwarantowany w każdym przebiegu.

## Uruchomienie (PowerShell z korzenia repo)
```powershell
dotnet run --project DotNet_ShowLab/04_RaceCondition/src/Demo.csproj -c Release
```

## Przebieg
1. Zapytaj studentów, czego się spodziewają.
2. Uruchom pokaz i porównaj pomiary lub odpowiedzi.
3. Omów wynik oraz ograniczenia eksperymentu.

## Interpretacja
Równoległa inkrementacja bez synchronizacji kontra Interlocked. Wynik błędny nie jest gwarantowany w każdym przebiegu.


