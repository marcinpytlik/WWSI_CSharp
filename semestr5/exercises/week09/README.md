# Tydzień 9 – Testy integracyjne

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. Pierwszy test API — 10 pkt

Napisz test WebApplicationFactory sprawdzający GET /health.

**Kryteria zaliczenia:**

- Factory.

- HttpClient.

- status 200.

- body.

## Ćwiczenie 2. Test 404 — 15 pkt

Dodaj test dla nieistniejącego zasobu.

**Kryteria zaliczenia:**

- Test endpointu.

- 404.

- niezależność danych.

## Ćwiczenie 3. Test POST — 20 pkt

Przetestuj poprawny i błędny POST wraz z walidacją.

**Kryteria zaliczenia:**

- 201 + 400.

- JSON request.

- asercje response.

## Ćwiczenie 4. Test z bazą testową — 25 pkt

Podmień konfigurację bazy na testową/InMemory/SQLite-in-memory i izoluj dane między testami.

**Kryteria zaliczenia:**

- Override DI.

- izolacja.

- deterministyczność.

- cleanup.

## Ćwiczenie 5. Pakiet integracyjny — 30 pkt

Przygotuj min. 15 testów CRUD + auth + validation + middleware, z własną WebApplicationFactory i helperami do autoryzacji.

**Kryteria zaliczenia:**

- 15 testów.

- izolowana baza.

- auth.

- przypadki happy/error.

- czytelna infrastruktura testowa.

---

**Suma: 100 pkt**
