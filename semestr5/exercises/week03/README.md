# Tydzień 3 – Walidacja modeli i DTO

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. DTO wejściowe — 10 pkt

Oddziel CreateUserRequest od encji/modelu domenowego.

**Kryteria zaliczenia:**

- Osobny DTO.

- mapowanie.

- brak nadmiarowych pól.

## Ćwiczenie 2. DataAnnotations — 15 pkt

Dodaj walidację Required, StringLength/Range/EmailAddress dla modelu wejściowego.

**Kryteria zaliczenia:**

- Min. 3 reguły.

- komunikaty.

- odrzucenie błędnych danych.

## Ćwiczenie 3. Walidacja biznesowa — 20 pkt

Dodaj regułę niedającą się opisać samą adnotacją, np. unikalność loginu lub daty.

**Kryteria zaliczenia:**

- Osobna logika.

- odpowiedni status.

- test scenariusza.

## Ćwiczenie 4. ProblemDetails — 25 pkt

Zwracaj błędy walidacji w spójnym formacie ProblemDetails/ValidationProblem.

**Kryteria zaliczenia:**

- Standardowy format.

- 400.

- szczegóły pól.

## Ćwiczenie 5. Pipeline walidacji — 30 pkt

Zaprojektuj wspólną walidację dla kilku endpointów bez kopiowania kodu; użyj filtra endpointu lub własnej warstwy.

**Kryteria zaliczenia:**

- Brak duplikacji.

- wielokrotne użycie.

- poprawne 400.

- czysty endpoint.

---

**Suma: 100 pkt**
