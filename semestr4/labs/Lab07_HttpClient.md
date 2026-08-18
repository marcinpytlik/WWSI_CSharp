# Lab07 HttpClient

- GET/POST
- retry i timeout
## Mapowanie na projekty (repo)
- `src/Task10_FetchApiAsync` + `tests/Task10_FetchApiAsync.Tests`
- `src/Task11_Fetch5Parallel` + `tests/Task11_Fetch5Parallel.Tests`
- `src/Task20_FetchJsonListFromApi` + `tests/Task20_FetchJsonListFromApi.Tests`

## Kroki (workflow)
1. Otwórz solution i przejdź do wskazanych projektów w `src/`.
2. Zaimplementuj funkcje/klasy tak, aby przechodziły testy w odpowiadających projektach `tests/`.
3. Uruchom testy dla danego laba i popraw do zielonego.

## Definition of Done (checklista)
- [ ] HttpClient jest wstrzyknięty (nie tworzysz go w środku metody).
- [ ] Błędy HTTP są wykrywane (`EnsureSuccessStatusCode`).
- [ ] Równoległe pobranie: 5 odpowiedzi wraca jako lista.
- [ ] JSON list: poprawna deserializacja i test.

## Weryfikacja (komendy)
```powershell
# cały repo
dotnet test

# tylko projekty z tego laba (przykład: odpal konkretne testy)
# dotnet test .\tests\<NazwaProjektu>.Tests\<NazwaProjektu>.Tests.csproj
```

## Bonus (opcjonalnie)
- Dodaj prosty retry (3 próby) i test z handlerem, który raz zwraca błąd.
- Dodaj timeout przez `CancellationTokenSource` (demo).

## Notatki / typowe pułapki
- Pisz kod „pod testy”: małe, czyste metody + brak efektów ubocznych tam, gdzie się da.
- Zawsze dodaj 1–2 testy na przypadki brzegowe: `null/empty/invalid` (jeśli ma sens).
