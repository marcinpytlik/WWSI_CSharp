# Lab02 Typy i składnia — szczegółowe laboratorium

**Czas:** 90–120 min · **Stack:** .NET 10, xUnit

## Cel

Rozróżnić typy wartościowe i referencyjne oraz bezpiecznie parsować dane wejściowe.

## Zadania

1. Napisz `ParseInt(string? s)` zwracający `int` albo rzucający `FormatException` / `ArgumentException` (pusty input). Nie używaj `Convert.ToInt32` jako jedynej ścieżki — `int.TryParse`.
2. Napisz `Describe(object? value)`: interpolacja z typem runtime (`GetType().Name`) i `null`.
3. Pokaż różnicę `checked` vs `unchecked` dla `int.MaxValue + 1` — test, że `checked` rzuca `OverflowException`.
4. Porównaj `string` (referencja, niemutowalna) z `int` (kopia przy przypisaniu) w dwóch asercjach.

## Testy (minimum)

```csharp
[Theory]
[InlineData("42", 42)]
[InlineData("-7", -7)]
public void ParseInt_ok(string s, int expected) => Assert.Equal(expected, Parser.ParseInt(s));

[Fact]
public void ParseInt_empty_throws() => Assert.Throws<ArgumentException>(() => Parser.ParseInt("  "));
```

## Kryteria

- [ ] brak `Parse` bez `TryParse` na danych użytkownika
- [ ] nullable `string?` obsłużony
- [ ] 5+ testów, w tym overflow
