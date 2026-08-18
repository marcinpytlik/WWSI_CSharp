# Lab06 Dziedziczenie — szczegółowe laboratorium

**Czas:** 120 min · **Stack:** .NET 10, xUnit

## Cel

Zaimplementować hierarchie `Shape` **bez łamania LSP** (kwadrat nie dziedziczy po prostokącie z setterami szerokości/wysokości).

## Zadania

1. `abstract class Shape { abstract double Area(); abstract double Perimeter(); }`
2. `Circle`, `Rectangle` — konstruktory z walidacją (> 0).
3. `Square` jako **osobna** klasa albo `Rectangle` tworzony fabryką `Rectangle.Square(side)` — uzasadnij w komentarzu.
4. Tablica `Shape[]` i suma pól — polimorfizm.

## Czego nie robić

Nie kopiuj antyprzykładu `Square : Rectangle` z seterami z `ProjektowanieOprogramowania/SOLID/03_LSP_Anti`.

## Kryteria

- [ ] `Area` kółka ≈ πr² (tolerancja 1e-6)
- [ ] ujemny promień rzuca
- [ ] test polimorficzny na 2+ kształtach
