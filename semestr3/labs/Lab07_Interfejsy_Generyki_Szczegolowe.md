# Lab07 Interfejsy i generyki — szczegółowe laboratorium

**Czas:** 120 min · **Stack:** .NET 10, xUnit

## Cel

Napisać `IRepository<T>` i implementację w pamięci z ograniczeniem `where T : class`.

## Zadania

1. `interface IRepository<T> { void Add(T item); T? GetById(Guid id); IReadOnlyList<T> All(); }`
2. Encja `Product` z `Id`, `Name`, `Price`.
3. `InMemoryRepository<T>` — `Add` odrzuca `null` i duplikat `Id` (jeśli T ma `Id` — zdefiniuj `interface IEntity { Guid Id { get; } }` i `where T : class, IEntity`).
4. Opcjonalnie: `IComparer<Product>` do sortowania po cenie.

## Kryteria

- [ ] repozytorium nie zależy od `Product` poza testami
- [ ] duplikat Id rzuca
- [ ] `GetById` zwraca null przy braku (nie wyjątek)
