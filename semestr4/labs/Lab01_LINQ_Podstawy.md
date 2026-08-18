# Lab01 LINQ Podstawy

- Where/Select/OrderBy
- projekcje
- ćwiczenia
## Mapowanie na projekty (repo)
- `src/Task01_EvenNumbers` + `tests/Task01_EvenNumbers.Tests`
- `src/Task02_Average` + `tests/Task02_Average.Tests`
- `src/Task05_SortStringsDesc` + `tests/Task05_SortStringsDesc.Tests`

## Kroki (workflow)
1. Otwórz solution i przejdź do wskazanych projektów w `src/`.
2. Zaimplementuj funkcje/klasy tak, aby przechodziły testy w odpowiadających projektach `tests/`.
3. Uruchom testy dla danego laba i popraw do zielonego.

## Definition of Done (checklista)
- [ ] Task01: zwracasz tylko parzyste (kolejność zachowana).
- [ ] Task02: średnia działa i pusty input jest obsłużony (zgodnie z testami).
- [ ] Task05: sortowanie malejące działa deterministycznie.
- [ ] Dopisane min. 2 przypadki brzegowe w testach (np. pusta tablica, duplikaty).

## Weryfikacja (komendy)
```powershell
# cały repo
dotnet test

# tylko projekty z tego laba (przykład: odpal konkretne testy)
# dotnet test .\tests\<NazwaProjektu>.Tests\<NazwaProjektu>.Tests.csproj
```

## Bonus (opcjonalnie)
- Dla Task05 porównaj `StringComparer.Ordinal` vs `InvariantCultureIgnoreCase` i opisz różnicę w komentarzu.
- Zrób wariant metod przyjmujących `IEnumerable<T>` (tam gdzie pasuje).

## Notatki / typowe pułapki
- Pisz kod „pod testy”: małe, czyste metody + brak efektów ubocznych tam, gdzie się da.
- Zawsze dodaj 1–2 testy na przypadki brzegowe: `null/empty/invalid` (jeśli ma sens).
