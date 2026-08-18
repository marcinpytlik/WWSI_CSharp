# Tydzień 6 – Relacje i Include

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. One-to-many — 10 pkt

Zamodeluj User 1→N TaskItem z kluczami obcymi i nawigacjami.

**Kryteria zaliczenia:**

- Relacja.

- FK.

- migracja.

- spójność.

## Ćwiczenie 2. Include — 15 pkt

Pobierz użytkownika wraz z zadaniami przez Include.

**Kryteria zaliczenia:**

- Include.

- 404.

- DTO odpowiedzi.

## Ćwiczenie 3. ThenInclude — 20 pkt

Dodaj trzeci poziom modelu, np. TaskItem→Comments, i użyj ThenInclude.

**Kryteria zaliczenia:**

- 3 poziomy.

- ThenInclude.

- poprawny wynik.

## Ćwiczenie 4. Many-to-many — 25 pkt

Zamodeluj TaskItem↔Tag wiele-do-wielu i endpoint przypisujący tag.

**Kryteria zaliczenia:**

- M:N.

- migracja.

- endpoint.

- brak duplikatów relacji.

## Ćwiczenie 5. Optymalizacja zapytań — 30 pkt

Przygotuj dwa endpointy: pełny widok grafu i projekcję Select do lekkiego DTO. Porównaj generowany SQL/log i opisz, kiedy użyć którego.

**Kryteria zaliczenia:**

- Include vs projection.

- AsNoTracking.

- analiza SQL.

- uzasadnienie wydajnościowe.

---

**Suma: 100 pkt**
