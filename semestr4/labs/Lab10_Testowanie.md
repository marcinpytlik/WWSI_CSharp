# Lab10 Testowanie

- xUnit + Moq
- testy repozytorium
## Mapowanie na projekty (repo)
- `src/Task21_IsPrimeTestsTarget` + `tests/Task21_IsPrimeTestsTarget.Tests`
- `src/Task22_MoqRepositoryTarget` + `tests/Task22_MoqRepositoryTarget.Tests`
- `src/Task30_JsonRoundtripTestTarget` + `tests/Task30_JsonRoundtripTestTarget.Tests`

## Kroki (workflow)
1. Otwórz solution i przejdź do wskazanych projektów w `src/`.
2. Zaimplementuj funkcje/klasy tak, aby przechodziły testy w odpowiadających projektach `tests/`.
3. Uruchom testy dla danego laba i popraw do zielonego.

## Definition of Done (checklista)
- [ ] IsPrime: testy parametryzowane (Theory).
- [ ] Moq: Setup + Verify.
- [ ] JSON roundtrip: serialize+deserialize i porównanie obiektu.
- [ ] Dopisane min. 2 testy negatywne w tym labie.

## Weryfikacja (komendy)
```powershell
# cały repo
dotnet test

# tylko projekty z tego laba (przykład: odpal konkretne testy)
# dotnet test .\tests\<NazwaProjektu>.Tests\<NazwaProjektu>.Tests.csproj
```

## Bonus (opcjonalnie)
- Dodaj `Trait("Category","Unit")` albo spójne nazewnictwo testów.

## Notatki / typowe pułapki
- Pisz kod „pod testy”: małe, czyste metody + brak efektów ubocznych tam, gdzie się da.
- Zawsze dodaj 1–2 testy na przypadki brzegowe: `null/empty/invalid` (jeśli ma sens).
