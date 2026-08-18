# Tydzień 4 – EF Core: DbContext i migracje

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. Pierwszy DbContext — 10 pkt

Dodaj DbContext i encję Product, skonfiguruj provider oraz connection string.

**Kryteria zaliczenia:**

- DbContext.

- DbSet.

- konfiguracja DI.

- brak hardcoded hasła.

## Ćwiczenie 2. Konfiguracja encji — 15 pkt

Skonfiguruj Product przez Fluent API: klucz, długości, precision i wymagane pola.

**Kryteria zaliczenia:**

- IEntityTypeConfiguration lub OnModelCreating.

- min. 4 reguły.

## Ćwiczenie 3. Pierwsza migracja — 20 pkt

Utwórz migrację i opisz w README polecenia add/update/remove migration.

**Kryteria zaliczenia:**

- Migracja.

- baza aktualizuje się.

- dokumentacja CLI.

## Ćwiczenie 4. Seed danych — 25 pkt

Dodaj deterministyczne dane startowe lub inicjalizator deweloperski.

**Kryteria zaliczenia:**

- Powtarzalność.

- brak duplikatów.

- rozdzielenie prod/dev.

## Ćwiczenie 5. Konfiguracja wielu środowisk — 30 pkt

Rozdziel ustawienia bazy dla Development i Production oraz opisz obsługę sekretów bez umieszczania ich w repo.

**Kryteria zaliczenia:**

- appsettings env.

- User Secrets/env vars.

- README.

- brak sekretów w Git.

---

**Suma: 100 pkt**
