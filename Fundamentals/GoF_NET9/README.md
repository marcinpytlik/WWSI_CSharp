# GoF — 24 wzorce projektowe w .NET 9 (C#)

Repozytorium zawiera **24** foldery ze wzorcami (23 oryginalne GoF + bonus *Null Object*). Każdy folder ma projekt `src` oraz testy `tests` (xUnit).

## Wymagania
- .NET SDK 9.0+
- VS Code (polecane rozszerzenia: C#, C# Dev Kit)

## Komendy (w katalogu repozytorium)
```pwsh
dotnet build
dotnet test
```
Możesz też wejść do konkretnego folderu i wykonać polecenia tylko dla danego wzorca.

## Struktura przykładowego folderu
```
01_Singleton/
  src/Singleton.csproj
  src/SingletonDemo.cs
  tests/Singleton.Tests.csproj
  tests/SingletonTests.cs
```
