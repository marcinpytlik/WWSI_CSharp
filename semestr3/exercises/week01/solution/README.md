# Rozwiązania wzorcowe – Tydzień 1 (.NET 10)

Projekt demonstracyjny do [ćwiczeń tygodnia 1](../README.md) z semestru 3.

## Uruchamianie

Wymagany .NET SDK 10.

```powershell
cd semestr3/exercises/week01/solution
dotnet restore Week01.Solutions.slnx
dotnet build Week01.Solutions.slnx
dotnet run --project Week01.Solutions/Week01.Solutions.csproj
```

Po uruchomieniu wybierz ćwiczenie od **1** do **5**; **0** kończy program.

## Omówienie na zajęciach

| Opcja | Ćwiczenie | Punkty | Elementy do omówienia |
|---|---|---:|---|
| 1 | Pierwsza aplikacja | 10 | Zmienne `string`, `Console`, interpolacja, `RuntimeInformation` |
| 2 | Kalkulator | 15 | `decimal`, walidacja `TryParse`, działania, dzielenie przez zero |
| 3 | Formatowanie | 20 | `DateTime.Today.Year`, format `N1`, interpolacja |
| 4 | Wizytówka CLI | 25 | `const`, zmienne lokalne, separatory |
| 5 | Analizator środowiska | 30 | `Environment`, `RuntimeInformation`, dostępne procesory logiczne |

**Ważne:** ćwiczenie 3 podaje tylko rok urodzenia, dlatego wylicza **wiek rocznikowy**. Dokładny wiek wymaga pełnej daty urodzenia. W ćwiczeniu 5 `Environment.ProcessorCount` zwraca liczbę procesorów logicznych dostępnych procesowi, niekoniecznie wszystkich fizycznych rdzeni komputera.

## Przebieg pokazu

1. Otwórz plik `Week01.Solutions.slnx` w Visual Studio lub VS Code.
2. Uruchom aplikację i przejdź kolejno przez opcje 1–5.
3. Pokaż walidację: pusty tekst, niepoprawną liczbę, dzielenie przez zero.
4. Omów rozdzielenie operacji wejścia (`ReadDecimal`, `ReadInt`) od obliczeń.
5. Zaproś studentów do rozbudowania aplikacji (np. o pełną datę urodzenia).

To **wersja referencyjna dla prowadzącego**, a nie jedyny poprawny sposób rozwiązania zadań.
