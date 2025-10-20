# AplikacjaSwitch

Trzy osobne klasy z `Main`:
- `App.HelloApp` – wita po imieniu
- `App.MathApp` – suma, różnica, iloczyn, iloraz
- `App.ParityApp` – sprawdza parzystość

## Wymagania
- .NET SDK 9.0
- VS Code (opcjonalnie)

## Uruchamianie

### 1) Z linii poleceń
Zamień `StartupObject` na potrzebną klasę:
```bash
dotnet build -p:StartupObject=App.HelloApp
dotnet run   -p:StartupObject=App.HelloApp
```

Dostępne wartości:
- `App.HelloApp`
- `App.MathApp`
- `App.ParityApp`

### 2) VS Code (Tasks)
`Terminal → Run Task → Run (wybierz klasę Main)` i wybierz klasę z listy.

## Kompilacja csc
Alternatywnie:
```bash
csc /t:exe /out:AplikacjaSwitch.exe /main:App.HelloApp Program.Hello.cs Program.Math.cs Program.Parity.cs
.\AplikacjaSwitch.exe
```
