# Lab08 Architektura

- warstwy: Core/Infra/UI
- kontrakty i DI
## Mapowanie na projekty (repo)
- `src/Task15_IRepositoryGeneric` + `tests/Task15_IRepositoryGeneric.Tests`
- `src/Task14_InMemoryProductRepo` + `tests/Task14_InMemoryProductRepo.Tests`
- `src/Task23_OrderServiceTotal` + `tests/Task23_OrderServiceTotal.Tests`
- `src/Task22_MoqRepositoryTarget` + `tests/Task22_MoqRepositoryTarget.Tests`

## Kroki (workflow)
1. Otwórz solution i przejdź do wskazanych projektów w `src/`.
2. Zaimplementuj funkcje/klasy tak, aby przechodziły testy w odpowiadających projektach `tests/`.
3. Uruchom testy dla danego laba i popraw do zielonego.

## Definition of Done (checklista)
- [ ] Repo kontrakt jest minimalny i sensowny.
- [ ] InMemory repo działa deterministycznie i ma testy CRUD.
- [ ] OrderService to czysta logika (bez IO).
- [ ] Task22 pokazuje DI/mocking i ma Verify w teście.

## Weryfikacja (komendy)
```powershell
# cały repo
dotnet test

# tylko projekty z tego laba (przykład: odpal konkretne testy)
# dotnet test .\tests\<NazwaProjektu>.Tests\<NazwaProjektu>.Tests.csproj
```

## Bonus (opcjonalnie)
- Dopisz krótki diagram w komentarzu (Core → Infra) w pliku laba.

## Notatki / typowe pułapki
- Pisz kod „pod testy”: małe, czyste metody + brak efektów ubocznych tam, gdzie się da.
- Zawsze dodaj 1–2 testy na przypadki brzegowe: `null/empty/invalid` (jeśli ma sens).
