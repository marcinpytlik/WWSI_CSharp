# Lab12 Powtorka

- quiz + refaktoryzacja
- checklisty jakości
## Mapowanie na projekty (repo)
- Wszystkie projekty `src/Task01..Task30` + testy `tests/*`

## Kroki (workflow)
1. Otwórz solution i przejdź do wskazanych projektów w `src/`.
2. Zaimplementuj funkcje/klasy tak, aby przechodziły testy w odpowiadających projektach `tests/`.
3. Uruchom testy dla danego laba i popraw do zielonego.

## Definition of Done (checklista)
- [ ] `dotnet test` przechodzi na czysto.
- [ ] Dodałeś po 1 przypadku brzegowym do 5 wybranych tasków.
- [ ] Zrobiłeś mini-refaktor (usunięcie duplikacji albo poprawa nazw).
- [ ] Dodałeś krótką notatkę „co poprawiłeś i dlaczego” na końcu tego laba.

## Weryfikacja (komendy)
```powershell
# cały repo
dotnet test

# tylko projekty z tego laba (przykład: odpal konkretne testy)
# dotnet test .\tests\<NazwaProjektu>.Tests\<NazwaProjektu>.Tests.csproj
```

## Bonus (opcjonalnie)
- Dodaj link w root README do `labs/INDEX.md` (Start here).

## Notatki / typowe pułapki
- Pisz kod „pod testy”: małe, czyste metody + brak efektów ubocznych tam, gdzie się da.
- Zawsze dodaj 1–2 testy na przypadki brzegowe: `null/empty/invalid` (jeśli ma sens).
