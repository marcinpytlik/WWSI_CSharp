# Rozwiązania wzorcowe – Tydzień 2 (.NET 10)

Kod do [ćwiczeń tygodnia 2](../README.md), semestr 3.

## Start

Wymagany .NET SDK 10.

```powershell
cd semestr3/exercises/week02/solution
dotnet restore Week02.Solutions.slnx
dotnet build Week02.Solutions.slnx
dotnet run --project Week02.Solutions/Week02.Solutions.csproj
```

Menu **1–5** uruchamia wybrane ćwiczenie, **0** kończy program.

| Opcja | Zadanie | Punkty | Na co zwrócić uwagę |
|---|---|---:|---|
| 1 | Typy liczbowe | 10 | `int`, `long`, `decimal`, `double`; dokładność `0.1 + 0.2` |
| 2 | Temperatura | 15 | °F = °C × 9/5 + 32, K = °C + 273,15 |
| 3 | Bezpieczne parsowanie | 20 | `int.TryParse`, `decimal.TryParse`, poprawne i błędne wejście |
| 4 | Koszyk | 25 | `decimal`, VAT, zaokrąglanie, format `C2` |
| 5 | Operatory | 30 | Arytmetyczne, porównania, logiczne, `??`, `??=` |

## Sugestia do demonstracji

1. Uruchom przykład 1 i omów dlaczego `double` nie powinien być domyślnym typem dla pieniędzy.
2. W konwerterze temperatur spróbuj `0`, `100` i `-273,15`; sprawdź wejście poniżej zera bezwzględnego.
3. W parsowaniu podaj najpierw tekst zamiast liczby, a potem prawidłowe wartości.
4. W koszyku przetestuj cenę `19,99`, stawkę `23` i trzy sztuki.
5. W operatorach pokaż dzielenie całkowite i różnicę między `??` a `??=`.

**Uwaga:** przykład 4 liczy VAT od sumy netto i zaokrągla do dwóch miejsc. W systemach produkcyjnych trzeba jednoznacznie ustalić zasady zaokrąglania.

Jest to przykład dla prowadzącego, a nie jedyna poprawna implementacja.
