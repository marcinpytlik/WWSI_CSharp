# Lab11 Projekt ToDo

- ToDo w JSON
- filtry LINQ + persystencja
## Mapowanie na projekty (repo)
- `src/Task28_TodoConsoleJson` + `tests/Task28_TodoConsoleJson.Tests`
- `src/Task29_TodoFilterByStatus` + `tests/Task29_TodoFilterByStatus.Tests`
- `src/Task30_JsonRoundtripTestTarget` + `tests/Task30_JsonRoundtripTestTarget.Tests`

## Kroki (workflow)
1. Otwórz solution i przejdź do wskazanych projektów w `src/`.
2. Zaimplementuj funkcje/klasy tak, aby przechodziły testy w odpowiadających projektach `tests/`.
3. Uruchom testy dla danego laba i popraw do zielonego.

## Definition of Done (checklista)
- [ ] ToDo zapisuje i odczytuje JSON (persist).
- [ ] Filtr po statusie działa i ma test.
- [ ] Format JSON jest stabilny (roundtrip test).
- [ ] Dopisany test: brak pliku → pusta lista.

## Weryfikacja (komendy)
```powershell
# cały repo
dotnet test

# tylko projekty z tego laba (przykład: odpal konkretne testy)
# dotnet test .\tests\<NazwaProjektu>.Tests\<NazwaProjektu>.Tests.csproj
```

## Bonus (opcjonalnie)
- Dodaj `UpdateStatus(Guid id, TodoStatus status)` + test.
- Dodaj sortowanie po dacie utworzenia + test.

## Notatki / typowe pułapki
- Pisz kod „pod testy”: małe, czyste metody + brak efektów ubocznych tam, gdzie się da.
- Zawsze dodaj 1–2 testy na przypadki brzegowe: `null/empty/invalid` (jeśli ma sens).
