# Lab02 LINQ Zaawansowane

- GroupBy/Join
- agregacje
- wydajność
## Mapowanie na projekty (repo)
- `src/Task03_GroupByAge` + `tests/Task03_GroupByAge.Tests`
- `src/Task04_JoinCollections` + `tests/Task04_JoinCollections.Tests`
- `src/Task23_OrderServiceTotal` + `tests/Task23_OrderServiceTotal.Tests`
- `src/Task29_TodoFilterByStatus` + `tests/Task29_TodoFilterByStatus.Tests`

## Kroki (workflow)
1. Otwórz solution i przejdź do wskazanych projektów w `src/`.
2. Zaimplementuj funkcje/klasy tak, aby przechodziły testy w odpowiadających projektach `tests/`.
3. Uruchom testy dla danego laba i popraw do zielonego.

## Definition of Done (checklista)
- [ ] GroupBy: grupy są poprawne i łatwe w użyciu.
- [ ] Join: prawidłowo łączysz po kluczu i zwracasz projekcję.
- [ ] Agregacje: suma zamówienia używa `decimal`.
- [ ] Filtrowanie ToDo: filtr zwraca tylko pasujące elementy.
- [ ] Dopisane testy: brak dopasowań w join + puste wejście.

## Weryfikacja (komendy)
```powershell
# cały repo
dotnet test

# tylko projekty z tego laba (przykład: odpal konkretne testy)
# dotnet test .\tests\<NazwaProjektu>.Tests\<NazwaProjektu>.Tests.csproj
```

## Bonus (opcjonalnie)
- Dla Join dopisz wariant `GroupJoin` (klient -> lista zamówień).
- Dopisz krótki komentarz o leniwej ewaluacji LINQ (kiedy `ToList()` ma znaczenie).

## Notatki / typowe pułapki
- Pisz kod „pod testy”: małe, czyste metody + brak efektów ubocznych tam, gdzie się da.
- Zawsze dodaj 1–2 testy na przypadki brzegowe: `null/empty/invalid` (jeśli ma sens).
