# Lab05 JSON XML

- System.Text.Json
- serializacja i walidacja
## Mapowanie na projekty (repo)
- `src/Task08_ReadJsonObject` + `tests/Task08_ReadJsonObject.Tests`
- `src/Task09_WriteUserJson` + `tests/Task09_WriteUserJson.Tests`
- `src/Task24_ProductPriceValidation` + `tests/Task24_ProductPriceValidation.Tests`
- `src/Task26_ReadXmlTitles` + `tests/Task26_ReadXmlTitles.Tests`
- `src/Task30_JsonRoundtripTestTarget` + `tests/Task30_JsonRoundtripTestTarget.Tests`

## Kroki (workflow)
1. Otwórz solution i przejdź do wskazanych projektów w `src/`.
2. Zaimplementuj funkcje/klasy tak, aby przechodziły testy w odpowiadających projektach `tests/`.
3. Uruchom testy dla danego laba i popraw do zielonego.

## Definition of Done (checklista)
- [ ] JSON read: deserializacja działa (Web defaults).
- [ ] JSON write: zapis + odczyt daje ten sam obiekt.
- [ ] Walidacja: ujemna cena rzuca wyjątek i ma test.
- [ ] XML: wyciągasz wszystkie `<title>` poprawnie.
- [ ] Dopisany test na niepoprawny JSON (negatywny scenariusz).

## Weryfikacja (komendy)
```powershell
# cały repo
dotnet test

# tylko projekty z tego laba (przykład: odpal konkretne testy)
# dotnet test .\tests\<NazwaProjektu>.Tests\<NazwaProjektu>.Tests.csproj
```

## Bonus (opcjonalnie)
- Dodaj `JsonPropertyName` w modelu (demo mapowania nazw pól).

## Notatki / typowe pułapki
- Pisz kod „pod testy”: małe, czyste metody + brak efektów ubocznych tam, gdzie się da.
- Zawsze dodaj 1–2 testy na przypadki brzegowe: `null/empty/invalid` (jeśli ma sens).
