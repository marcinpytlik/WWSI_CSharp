# Lab09 Wzorce

- Repozytorium, Fabryka, Singleton
## Mapowanie na projekty (repo)
- `src/Task14_InMemoryProductRepo` (Repository) + testy
- `src/Task15_IRepositoryGeneric` (abstrakcja repo) + testy
- `src/Task16_SingletonPattern` + testy
- `src/Task17_ShapeFactory` + testy

## Kroki (workflow)
1. Otwórz solution i przejdź do wskazanych projektów w `src/`.
2. Zaimplementuj funkcje/klasy tak, aby przechodziły testy w odpowiadających projektach `tests/`.
3. Uruchom testy dla danego laba i popraw do zielonego.

## Definition of Done (checklista)
- [ ] Wskazujesz w kodzie, gdzie jest wzorzec i po co (komentarz 1–2 zdania).
- [ ] Singleton to pojedyncza instancja (test: `Same`).
- [ ] Factory: poprawne typy i testy na area.
- [ ] Repo: testy add/get/remove.

## Weryfikacja (komendy)
```powershell
# cały repo
dotnet test

# tylko projekty z tego laba (przykład: odpal konkretne testy)
# dotnet test .\tests\<NazwaProjektu>.Tests\<NazwaProjektu>.Tests.csproj
```

## Bonus (opcjonalnie)
- Dla Singleton dopisz krótką notatkę: kiedy NIE używać (testowalność).

## Notatki / typowe pułapki
- Pisz kod „pod testy”: małe, czyste metody + brak efektów ubocznych tam, gdzie się da.
- Zawsze dodaj 1–2 testy na przypadki brzegowe: `null/empty/invalid` (jeśli ma sens).
