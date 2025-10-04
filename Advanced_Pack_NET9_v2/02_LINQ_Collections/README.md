# 02 — LINQ & Collections
- Operatory LINQ, pułapki alokacji, `Span<T>` w prostych scenariuszach.


## Zadania domowe
- [ ] Zaimplementuj własny `SelectMany` dla `IEnumerable<T>` (ćwiczenie) i porównaj z LINQ.
- [ ] Porównaj alokacje: `ToList()` vs `Span<T>` w sumowaniu (BenchmarkDotNet).
- [ ] Napisz rozszerzenie `Chunked<T>(int size)` i przetestuj przypadki brzegowe.
- [ ] Przeanalizuj złożoność `OrderBy().ThenBy()` na dużych danych.
