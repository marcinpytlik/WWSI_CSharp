# Lab03 Sterowanie — szczegółowe laboratorium

**Data:** 2025-10-04  
**Czas trwania:** 90–120 min  
**Poziom:** początkujący → średniozaawansowany  
**Wymagane oprogramowanie:** VS Code, .NET SDK 8.0+, Git, PowerShell 7+

## Cel
Po tym labie będziesz umieć:
- Uruchamiać projekty .NET 8 z linii poleceń (`dotnet`).
- Stosować kluczowe elementy tematu: if/switch, for/while/foreach, ćwiczenia z warunków.
- Pisać czytelny kod zgodnie z konwencjami i mieć automatyczne testy tam, gdzie to ma sens.

## Repo i struktura katalogów
```
/Lab03_Sterowanie/
  README.md
  src/
  tests/
  .vscode/
    launch.json
    tasks.json
```

## Przygotowanie (10 min)
1. Utwórz katalog labu:  
   ```pwsh
   mkdir Lab03_Sterowanie; cd Lab03_Sterowanie
   git init
   ```
2. Stwórz projekt konsolowy i testy xUnit:  
   ```pwsh
   dotnet new console -n src -f net8.0
   dotnet new xunit -n tests -f net8.0
   dotnet sln new Lab03_Sterowanie.sln
   dotnet sln add src/src.csproj tests/tests.csproj
   dotnet add tests/tests.csproj reference src/src.csproj
   ```
3. Dodaj formatowanie i analizatory:  
   ```pwsh
   dotnet new editorconfig
   dotnet new gitignore
   dotnet tool install -g dotnet-format
   ```

## Konfiguracja VS Code (5 min)
Utwórz plik `.vscode/tasks.json`:
```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "command": "dotnet",
      "args": ["build"],
      "type": "process",
      "group": "build",
      "problemMatcher": "$msCompile"
    },
    {
      "label": "test",
      "command": "dotnet",
      "args": ["test"],
      "type": "process",
      "group": "test",
      "problemMatcher": "$msCompile"
    },
    {
      "label": "format",
      "command": "dotnet",
      "args": ["format"],
      "type": "process"
    }
  ]
}
```

`launch.json` dla debugowania `src`:
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Launch (src)",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/src/bin/Debug/net8.0/src.dll",
      "preLaunchTask": "build",
      "cwd": "${workspaceFolder}/src"
    }
  ]
}
```

## Zadania główne
Na bazie skrótu z pliku wejściowego wykonaj następujące kroki (wzorowane na punktach wstępnych):
# Lab03 Sterowanie

- if/switch
- for/while/foreach
- ćwiczenia z warunków


---

### Krok 1. Implementacja
1. Zaimplementuj kod w `Program.cs` oraz w nowych plikach klasy/rekordów w `src/` tak, aby pokryć wszystkie wypunktowane elementy.
2. Stosuj *małe, testowalne jednostki* (funkcje statyczne lub metody klas).

### Krok 2. Testy jednostkowe
Dodaj testy w `tests/` używając xUnit:
```csharp
using Xunit;

public class SampleTests
{
    [Theory]
    [InlineData(2, 2, 4)]
    [InlineData(-1, 1, 0)]
    public void Add_Works(int a, int b, int expected)
    {
        Assert.Equal(expected, a + b);
    }
}
```

### Krok 3. Interakcja z użytkownikiem (I/O)
- Program powinien przyjmować **argumenty wiersza poleceń** oraz **odczyt ze standardowego wejścia**.
- Zadbaj o walidację i zwięzłe komunikaty o błędach.

### Krok 4. Obsługa błędów
- Dla scenariuszy wyjątków przygotuj oddzielne metody, które rzucają i logują wyjątki.
- Dodaj co najmniej **3 testy**, które potwierdzają prawidłową obsługę błędów.

### Krok 5. Jakość
- Uruchom `dotnet format` oraz upewnij się, że `dotnet build` i `dotnet test` kończą się sukcesem.
- Dodaj `README.md` z krótką instrukcją uruchomienia: `dotnet run --project src -- --help`.

## Mini‑projekty (wybierz 1)
- **A:** Rozszerz funkcjonalność o zapis/odczyt do pliku JSON (`System.Text.Json`).  
- **B:** Dodaj prosty *benchmark* wybranej funkcji (`BenchmarkDotNet`).  
- **C:** Stwórz wersję *async* z `Task` i `await`.

## Kryteria akceptacji
- [ ] Kompiluje się bez błędów i ostrzeżeń (W,A).  
- [ ] 80%+ linii pokryte testami w projekcie `tests`.  
- [ ] Program obsługuje niepoprawne dane wejściowe bez crasha.  
- [ ] `README.md` zawiera przykłady użycia i sekcję *Troubleshooting*.  
- [ ] Zastosowano konwencje nazewnicze (PascalCase dla typów, camelCase dla parametrów).

## Weryfikacja (checklist)
- [ ] `dotnet build`  
- [ ] `dotnet test`  
- [ ] Uruchomienie z przykładowymi danymi wejściowymi  
- [ ] Test przypadków brzegowych (puste, null, very large)

## Sprzątanie
```pwsh
dotnet clean
git add .
git commit -m "Finish Lab03 Sterowanie"
```

---

## Dla ambitnych
- Dodaj *CI* w GitHub Actions: workflow z `dotnet build`, `test`, `format`.
- Dodaj *global tool manifest* i użyj `dotnet format` z manifestu.

