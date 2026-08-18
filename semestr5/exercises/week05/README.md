# Tydzień 5 – CRUD z EF Core

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. Create — 10 pkt

Zaimplementuj POST /users zapisujący użytkownika przez EF Core i zwracający CreatedAtRoute/Created.

**Kryteria zaliczenia:**

- async.

- SaveChangesAsync.

- 201.

- lokalizacja zasobu.

## Ćwiczenie 2. Read — 15 pkt

Dodaj GET all i GET by id z AsNoTracking dla operacji tylko do odczytu.

**Kryteria zaliczenia:**

- 2 endpointy.

- AsNoTracking.

- 404.

## Ćwiczenie 3. Update — 20 pkt

Dodaj PUT/PATCH aktualizujący wybrane dane użytkownika i obsługujący brak rekordu.

**Kryteria zaliczenia:**

- Async EF.

- walidacja.

- 404/200 lub 204.

## Ćwiczenie 4. Delete — 25 pkt

Dodaj DELETE i zabezpiecz usunięcie nieistniejącego rekordu.

**Kryteria zaliczenia:**

- Remove.

- SaveChangesAsync.

- 404/204.

## Ćwiczenie 5. CRUD produkcyjny — 30 pkt

Dodaj paginację, filtrowanie, sortowanie oraz DTO; nie zwracaj encji EF bezpośrednio z API.

**Kryteria zaliczenia:**

- DTO.

- pagination metadata.

- filtry.

- sort.

- async.

- brak overposting.

---

**Suma: 100 pkt**
