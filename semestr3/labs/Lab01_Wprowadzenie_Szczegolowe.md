# Lab01 Wprowadzenie — szczegółowe laboratorium

**Czas:** 90–120 min · **Stack:** .NET 10, xUnit, VS Code  
**Wymagania:** .NET SDK 10.x, Git

## Cel

Uruchomić pierwszy projekt konsolowy i zrozumieć cykl `restore → build → run → test`.
W repo jest gotowa implementacja: `Lab01_Wprowadzenie/`.

## Gotowy projekt (zalecane)

```bash
cd semestr3/labs/Lab01_Wprowadzenie
dotnet test
dotnet run --project src -- greet WWSI
dotnet run --project src -- add 2 3
```

Polecenia: `greet [name]`, `add <a> <b>` (albo dwie liczby ze stdin), `to-json`, `--help`.
Kody wyjścia: 0 OK, 1 usage, 2 błąd wykonania.

## Zadanie własne (jeśli startujesz od zera)

```bash
dotnet new console -n Lab01 -f net10.0
dotnet new xunit -n Lab01.Tests -f net10.0
dotnet add Lab01.Tests reference Lab01
```

Wydziel logikę z `Main` (jak `App.Run` w referencji), żeby dało się testować bez konsoli.

## Kryteria

- [ ] `dotnet build` i `dotnet test` przechodzą
- [ ] program obsługuje `--help`
- [ ] niepoprawne dane nie crashują procesu (kod ≠ 0, komunikat na stderr)
