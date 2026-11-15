# Semestr 3 — Tydzień 4: rozwiązania wzorcowe

Projekt konsolowy **.NET 10** realizuje [ćwiczenia tygodnia 4](../README.md), łącznie **100 pkt**. Zachowano menu 1–5 jak w tygodniu 3.

## Uruchomienie

```powershell
cd semestr3/exercises/week04/solution
dotnet restore Week04.Solutions.slnx
dotnet build Week04.Solutions.slnx
dotnet run --project Week04.Solutions/Week04.Solutions.csproj
```

## Mapowanie zadań

| Opcja | Ćwiczenie | Punkty | Weryfikacja |
|---|---|---:|---|
| 1 | Metody matematyczne | 10 | Cztery metody, typ `decimal`, dzielenie przez zero |
| 2 | Walidator dodatniej liczby | 15 | `out`, powtarzanie pytania po wpisaniu `abc`, `0`, `-1` |
| 3 | Przeciążona `Format` | 20 | Trzy przeciążenia (`string`, `int`, `decimal`), kultura `pl-PL` |
| 4 | Statystyki tablicy | 25 | Jedno wywołanie, krotka z min/max/sum/avg, pusta tablica |
| 5 | Refaktoryzacja kalkulatora | 30 | Wejście, walidacja, obliczenie, prezentacja; dzielenie przez zero |

## Scenariusze weryfikacji

- **1:** `10`, `2` → dodawanie 12, odejmowanie 8, mnożenie 20, dzielenie 5. Wariant `b=0` nie wykonuje dzielenia.
- **2:** `abc`, `0`, `-5`, `7` → ponowienie pytania, ostatecznie wartość 7.
- **3:** `"  WWSI C#  "` → `"WWSI C#"`; `1234567` → `1 234 567` (separator grup zależy od kultury); `1234.567m` → dwie cyfry po przecinku.
- **4:** `4 8 -2 0` → min -2, max 8, suma 10, średnia 2,50; pusty wiersz → czytelny komunikat.
- **5:** `12,5`, `+`, `2,5` → `15`; `/` przez `0` → błąd, program działa dalej.

**Decyzje dydaktyczne:** Suma tablicy korzysta z `long`, aby nie przepełnić `int` przy typowych danych. Niepoprawne elementy tablicy są odrzucane. Pętla walidacyjna kończy się także przy EOF. Kalkulator wykorzystuje te same metody matematyczne co ćwiczenie 1. Separator dziesiętny w wejściu to przecinek (`pl-PL`).
