# Lab09 Wyjątki — szczegółowe laboratorium

**Czas:** 90 min · **Stack:** .NET 10, xUnit

## Cel

Rzucać wąskie wyjątki i testować je. Nie połykać błędów pustym `catch`.

## Zadania

1. `class CsvFormatException : FormatException` z właściwością `LineNumber`.
2. `CsvParser.ParseLine(string line, int lineNumber)` — oczekuj 3 kolumn; inaczej `CsvFormatException`.
3. `ParseFile(string content)` dzieli na linie, zbiera błędy **albo** przerywa przy pierwszym (wybierz jedną strategię i opisz w README).
4. Filtr `catch (CsvFormatException ex) when (ex.LineNumber == 1)`.

## Kryteria

- [ ] brak `catch (Exception) { }` bez `throw`
- [ ] test `Assert.Throws<CsvFormatException>` sprawdza `LineNumber`
- [ ] poprawna linia `"a,b,c"` działa
