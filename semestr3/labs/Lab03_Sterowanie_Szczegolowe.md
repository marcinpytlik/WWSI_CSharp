# Lab03 Sterowanie — szczegółowe laboratorium

**Czas:** 90–120 min · **Stack:** .NET 10, xUnit

## Cel

Zastąpić zagnieżdżone `if` wyrażeniem `switch` i napisać pętle bez off-by-one.

## Zadania

1. `FizzBuzz(int n)` → `IEnumerable<string>` dla 1..n (3 Fizz, 5 Buzz, 15 FizzBuzz, inaczej liczba).
2. `Grade(int percent)` jako **switch expression** (0–100; poza zakresem wyjątek). Progi: 90 A, 75 B, 60 C, 50 D, inaczej F.
3. `IndexOf(int[] data, int value)` pętlą `for` — bez LINQ. Brak: `-1`.
4. Wczytaj liczby aż do pustej linii (`while`), zwróć sumę. Testuj przez `StringReader`.

## Pułapki

- `switch` na `int` z zakresami: `>= 90 and <= 100`
- FizzBuzz: najpierw 15, potem 3 i 5
- pusta tablica w `IndexOf`

## Kryteria

- [ ] FizzBuzz ma test Theory na 1, 3, 5, 15
- [ ] ocena poza 0–100 rzuca
- [ ] brak `goto`
