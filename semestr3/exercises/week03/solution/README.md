# Semestr 3 — Tydzień 3: rozwiązania wzorcowe

Projekt konsolowy .NET 10 realizuje [ćwiczenia tygodnia 3](../README.md) (100 pkt).

```powershell
cd semestr3/exercises/week03/solution
dotnet build Week03.Solutions.slnx
dotnet run --project Week03.Solutions/Week03.Solutions.csproj
```

Menu 1–5 wybiera ćwiczenie, 0 zamyka aplikację.

| Opcja | Ćwiczenie | Punkty | Przykłady |
|---|---|---:|---|
| 1 | Ocena z punktów | 10 | 0, 50, 51, 90, 91, 100 i błędne wejście |
| 2 | Kalkulator | 15 | +, -, *, /; dzielenie przez 0, nieznany operator |
| 3 | Tabliczka mnożenia | 20 | Dwie zagnieżdżone pętle od 1 do 10 |
| 4 | Zgadnij liczbę | 25 | Losowanie, podpowiedzi, licznik poprawnych prób |
| 5 | Analizator liczb | 30 | Parzyste 50, nieparzyste 50, podzielne przez 3: 33, przez 5: 20 |

**Uwaga:** treść zadania 1 nie narzuca progów ocen; przyjęto: 0–50 → 2,0; 51–60 → 3,0; 61–70 → 3,5; 71–80 → 4,0; 81–90 → 4,5; 91–100 → 5,0. W analizatorze liczba może należeć do kilku kategorii jednocześnie. To jedna z możliwych realizacji zadań.
