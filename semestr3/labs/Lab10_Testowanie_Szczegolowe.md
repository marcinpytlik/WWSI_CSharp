# Lab10 Testowanie — szczegółowe laboratorium

**Czas:** 120 min · **Stack:** .NET 10, xUnit

## Cel

Pisać testy **najpierw albo równolegle**, w stylu AAA, bez testowania frameworka.

## Zadania

1. Kata: `Roman.ToInt(string)` dla I, IV, IX, XL, XC, C (wystarczy ten podzbiór) — Theory.
2. Zły input (`""`, `"Z"`, `null`) → wyjątek.
3. Nazwy: `ToInt_IV_Returns4`.
4. Nie testuj `1 + 1` — testuj **swoją** logikę. Wzorzec I/O: patrz `Lab01_Wprowadzenie/tests`.

## Kryteria

- [ ] minimum 8 przypadków Theory + 2 wyjątki
- [ ] testy są deterministyczne (bez `DateTime.Now` bez zegara)
- [ ] `dotnet test` zielony
