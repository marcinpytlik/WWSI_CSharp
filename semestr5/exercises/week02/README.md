# Tydzień 2 – Routing i parametry

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. Route parameter — 10 pkt

Dodaj GET /users/{id:int} z walidacją identyfikatora i NotFound.

**Kryteria zaliczenia:**

- Constraint int.

- 400/404/200.

- czytelna odpowiedź.

## Ćwiczenie 2. Query parameters — 15 pkt

Dodaj endpoint filtrujący kolekcję po search, page i pageSize.

**Kryteria zaliczenia:**

- Query binding.

- wartości domyślne.

- walidacja zakresów.

## Ćwiczenie 3. Route groups — 20 pkt

Podziel API na grupy /users i /tasks z osobnymi endpointami.

**Kryteria zaliczenia:**

- MapGroup.

- organizacja.

- brak konfliktów tras.

## Ćwiczenie 4. Typed results — 25 pkt

Przepisz wybrane endpointy na TypedResults/Results z jawnie opisanymi wariantami odpowiedzi.

**Kryteria zaliczenia:**

- TypedResults.

- min. 3 typy odpowiedzi.

- poprawne kody.

## Ćwiczenie 5. Wersjonowanie kontraktu — 30 pkt

Zaprojektuj /api/v1/products oraz /api/v2/products z różnym DTO, bez duplikowania całej logiki domenowej.

**Kryteria zaliczenia:**

- 2 wersje tras.

- osobne DTO.

- współdzielona logika.

- zgodność wsteczna.

---

**Suma: 100 pkt**
