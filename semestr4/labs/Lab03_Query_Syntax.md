# Lab03 Query Syntax

- składnia zapytaniowa vs metodowa
## Mapowanie na projekty (repo)
- `src/Task03_GroupByAge` — dodaj metodę `GroupByAge_QuerySyntax(...)` + test porównujący z wersją metodową
- `src/Task04_JoinCollections` — dodaj metodę `Join_QuerySyntax(...)` + test porównujący
- `src/Task05_SortStringsDesc` — dodaj metodę `Sort_QuerySyntax(...)` + test porównujący

## Kroki (workflow)
1. Otwórz solution i przejdź do wskazanych projektów w `src/`.
2. Zaimplementuj funkcje/klasy tak, aby przechodziły testy w odpowiadających projektach `tests/`.
3. Uruchom testy dla danego laba i popraw do zielonego.

## Definition of Done (checklista)
- [ ] Dodałeś alternatywne metody w query syntax (osobne metody).
- [ ] Testy porównują wyniki (metodowa vs query) i są identyczne.
- [ ] Kod jest czytelny: sensowne nazwy zmiennych w `from ... in ...`.

## Weryfikacja (komendy)
```powershell
# cały repo
dotnet test

# tylko projekty z tego laba (przykład: odpal konkretne testy)
# dotnet test .\tests\<NazwaProjektu>.Tests\<NazwaProjektu>.Tests.csproj
```

## Bonus (opcjonalnie)
- Dla join użyj `let` w query syntax (mały precompute).

## Notatki / typowe pułapki
- Pisz kod „pod testy”: małe, czyste metody + brak efektów ubocznych tam, gdzie się da.
- Zawsze dodaj 1–2 testy na przypadki brzegowe: `null/empty/invalid` (jeśli ma sens).
