# Program C# – semestr 3–5 (WWSI)

Materiały dydaktyczne C# / .NET 10 podzielone na trzy semestry oraz blok
projektowania oprogramowania (OOP, SOLID, GRASP, GoF).

## Wymagania

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- C# 14 (domyślna wersja języka dla `net10.0`)
- Visual Studio Code / Visual Studio / Rider
- Git

`global.json` w korzeniu repo wymusza SDK 10.x (`rollForward: latestFeature`).

## Punktacja

Jedno źródło prawdy: [PUNKTACJA.md](PUNKTACJA.md).

- ćwiczenia tygodniowe: **1000 pkt**, próg **700 pkt (70%)**
- mini-projekt: zaliczenie osobno
- dziennik ocen: szablony w `Gradebook/templates/` (wypełnionych arkuszy nie commitujemy)

## Semestr 3 – Podstawy C#

- [README](semestr3/README.md) · [Syllabus](semestr3/Semestr3_Syllabus.md) · [Plan](semestr3/Semestr3_PlanTygodniowy.md)
- laboratoria: `semestr3/labs/` (referencyjny kod: `Lab01_Wprowadzenie/`)
- ćwiczenia: `semestr3/exercises/`
- zadania domowe: `semestr3/tasks/`
- ściąga: `semestr3/cheat-sheets/CSharp_Podstawy.md`

## Semestr 4 – LINQ, IO, Async, architektura

- [README](semestr4/README.md) · [Syllabus](semestr4/Semestr4_Syllabus.md) · [Plan](semestr4/Semestr4_PlanTygodniowy.md)
- laboratoria + 30 kata z testami: `semestr4/labs/` (`LinqDotnet10Challenges.sln`)
- ćwiczenia: `semestr4/exercises/`
- zadania domowe: `semestr4/tasks/`
- ściąga: `semestr4/cheat-sheets/Linq_IO_Async.md`

## Semestr 5 – ASP.NET Minimal API + EF Core + JWT

- [README](semestr5/README.md) · [Syllabus](semestr5/Semestr5_Syllabus.md) · [Plan](semestr5/Semestr5_PlanTygodniowy.md)
- laboratoria: `semestr5/labs/` (referencyjny kod: `Lab01_MinimalAPI/`)
- ćwiczenia: `semestr5/exercises/`
- zadania domowe: `semestr5/tasks/`
- ściąga: `semestr5/cheat-sheets/ASP_EF_Sciaga.md`

## Projektowanie oprogramowania

- [OOP](ProjektowanieOprogramowania/OOP/README.md)
- [SOLID](ProjektowanieOprogramowania/SOLID/README.md)
- [GRASP](ProjektowanieOprogramowania/GRASP/README.md)
- [GoF (24 wzorce)](ProjektowanieOprogramowania/GoF/README.md)

## Demonstracje (10 mini-projektów)

Gotowe przykłady do pokazania na sali, od CLI do JWT: [demonstracje/README.md](demonstracje/README.md).

```bash
dotnet run --project demonstracje/01_KalkulatorCli/src -- add 2 3
dotnet run --project demonstracje/07_NotesApi/src
```

## Solution

Jedyna solution w korzeniu: `WWSI_CSharp_NET10.sln` (wszystkie projekty `.csproj`).

```bash
dotnet restore WWSI_CSharp_NET10.sln
dotnet build WWSI_CSharp_NET10.sln
dotnet test WWSI_CSharp_NET10.sln --no-build
```

Przykładowe laboratoria:

```bash
dotnet run --project semestr3/labs/Lab01_Wprowadzenie/src -- --help
dotnet run --project semestr5/labs/Lab01_MinimalAPI/src/Api
```

Osobna solution laboratoriów semestru 4: `semestr4/labs/LinqDotnet10Challenges.sln`.

## Literatura i licencja

- [Literatura](Literatura/README.md)
- [LICENSE](LICENSE) (MIT)
- [Migracja do .NET 10](MIGRATION_NET10.md)
