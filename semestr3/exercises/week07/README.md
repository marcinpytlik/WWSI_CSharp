# Tydzień 7 – Interfejsy, generyki i kolekcje podstawowe

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. Interfejs ILogger — 10 pkt

Zdefiniuj ILogger z Log(string) i implementacje ConsoleLogger oraz MemoryLogger.

**Kryteria zaliczenia:**

- Interfejs.

- 2 implementacje.

- wymienne użycie.

## Ćwiczenie 2. Generyczny Box<T> — 15 pkt

Napisz Box<T> przechowujący Value oraz metodę Describe. Przetestuj dla int, string i Person.

**Kryteria zaliczenia:**

- Typ generyczny.

- 3 instancje.

- brak object/cast.

## Ćwiczenie 3. Repozytorium in-memory — 20 pkt

Zaprojektuj IRepository<T> z Add, Remove, GetAll i implementację InMemoryRepository<T>.

**Kryteria zaliczenia:**

- Interfejs generyczny.

- kolekcja.

- 3 operacje.

## Ćwiczenie 4. Katalog studentów — 25 pkt

Przechowuj studentów w List<Student>; dodawaj, usuwaj, wyszukuj i sortuj po nazwisku.

**Kryteria zaliczenia:**

- List<T>.

- CRUD podstawowy.

- sortowanie.

## Ćwiczenie 5. Generyczny magazyn — 30 pkt

Zaprojektuj Inventory<T> dla elementów mających identyfikator (np. przez interfejs IIdentifiable). Dodaj wyszukiwanie po Id i kontrolę duplikatów.

**Kryteria zaliczenia:**

- Constraint/interfejs.

- generyczność.

- walidacja duplikatów.

- wyszukiwanie.

---

**Suma: 100 pkt**
