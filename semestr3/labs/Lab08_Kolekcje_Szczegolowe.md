# Lab08 Kolekcje — szczegółowe laboratorium

**Czas:** 120 min · **Stack:** .NET 10, xUnit

## Cel

Wybrać strukturę danych świadomie: lista vs słownik vs zbiór.

## Zadania

1. `WordCount(string text)` → `Dictionary<string,int>` (case-insensitive, ignoruj pustkę).
2. `UniquePreserveOrder(IEnumerable<string> items)` na `HashSet` + lista.
3. Symulacja kolejki drukarki: `Queue<Job>`, `Enqueue` / `Dequeue`.
4. Porównaj złożoność: wyszukiwanie w `List` vs `HashSet` — krótki komentarz w kodzie (nie mikrobenchmark obowiązkowy).

## Kryteria

- [ ] `"A a A"` → `a: 3`
- [ ] kolejność unikalnych zachowana
- [ ] `Dequeue` na pustej kolejce rzuca `InvalidOperationException`
