# Lab06 Async Await

- Task, WhenAll
- obsługa wyjątków async
## Mapowanie na projekty (repo)
- `src/Task12_AsyncExceptionHandling` + `tests/Task12_AsyncExceptionHandling.Tests`
- `src/Task13_TaskDelayLogging` + `tests/Task13_TaskDelayLogging.Tests`
- `src/Task11_Fetch5Parallel` + `tests/Task11_Fetch5Parallel.Tests` (aspekt Task.WhenAll)

## Kroki (workflow)
1. Otwórz solution i przejdź do wskazanych projektów w `src/`.
2. Zaimplementuj funkcje/klasy tak, aby przechodziły testy w odpowiadających projektach `tests/`.
3. Uruchom testy dla danego laba i popraw do zielonego.

## Definition of Done (checklista)
- [ ] Bezpieczne opakowanie wyjątków w wynik (Result).
- [ ] Task.Delay + log czasu (minimum 3 wpisy).
- [ ] Task.WhenAll: test potwierdza 5 wyników.
- [ ] Dopisany test negatywny (wyjątek) dla TryAsync.

## Weryfikacja (komendy)
```powershell
# cały repo
dotnet test

# tylko projekty z tego laba (przykład: odpal konkretne testy)
# dotnet test .\tests\<NazwaProjektu>.Tests\<NazwaProjektu>.Tests.csproj
```

## Bonus (opcjonalnie)
- Dodaj `CancellationToken` do metod i test na anulowanie (opcjonalnie).

## Notatki / typowe pułapki
- Pisz kod „pod testy”: małe, czyste metody + brak efektów ubocznych tam, gdzie się da.
- Zawsze dodaj 1–2 testy na przypadki brzegowe: `null/empty/invalid` (jeśli ma sens).
