# Lab04 IO Plik

- File/FileStream
- CSV read/write
## Mapowanie na projekty (repo)
- `src/Task06_ReadCsv` + `tests/Task06_ReadCsv.Tests`
- `src/Task18_CopyFileStreamReaderWriter` + `tests/Task18_CopyFileStreamReaderWriter.Tests`
- `src/Task19_SumNumbersLargeFile` + `tests/Task19_SumNumbersLargeFile.Tests`
- `src/Task25_FileLogger` + `tests/Task25_FileLogger.Tests`

## Kroki (workflow)
1. Otwórz solution i przejdź do wskazanych projektów w `src/`.
2. Zaimplementuj funkcje/klasy tak, aby przechodziły testy w odpowiadających projektach `tests/`.
3. Uruchom testy dla danego laba i popraw do zielonego.

## Definition of Done (checklista)
- [ ] CSV: poprawnie czytasz wiersze i opcjonalnie pomijasz nagłówek.
- [ ] Copy: plik docelowy ma identyczną zawartość jak źródłowy.
- [ ] Sum: używasz `File.ReadLines` (streaming).
- [ ] Logger: dopisuje do pliku (append) i nie gubi wpisów.
- [ ] Dopisane testy na: pusty plik / brak pliku (tam gdzie ma sens).

## Weryfikacja (komendy)
```powershell
# cały repo
dotnet test

# tylko projekty z tego laba (przykład: odpal konkretne testy)
# dotnet test .\tests\<NazwaProjektu>.Tests\<NazwaProjektu>.Tests.csproj
```

## Bonus (opcjonalnie)
- W CSV dodaj obsługę separatora `;` + test.
- W Sum dodaj test na mieszane dane (liczby + śmieci).

## Notatki / typowe pułapki
- Pisz kod „pod testy”: małe, czyste metody + brak efektów ubocznych tam, gdzie się da.
- Zawsze dodaj 1–2 testy na przypadki brzegowe: `null/empty/invalid` (jeśli ma sens).
