# Tydzień 1 – Minimal API

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. Hello API — 10 pkt

Utwórz Minimal API .NET 10 z endpointem GET /hello zwracającym obiekt JSON.

**Kryteria zaliczenia:**

- Projekt działa.

- endpoint.

- JSON.

- status 200.

## Ćwiczenie 2. Health check — 15 pkt

Dodaj GET /health zwracający status, timestamp i wersję aplikacji.

**Kryteria zaliczenia:**

- Endpoint.

- UTC timestamp.

- wersja.

- poprawny kontrakt.

## Ćwiczenie 3. Endpoint konfiguracji — 20 pkt

Dodaj konfigurację z appsettings.json i endpoint pokazujący bezpieczny podzbiór ustawień.

**Kryteria zaliczenia:**

- Options/config.

- brak sekretów.

- DTO odpowiedzi.

## Ćwiczenie 4. Grupowanie endpointów — 25 pkt

Zorganizuj endpointy przez MapGroup i dodaj wspólny prefiks /api/v1.

**Kryteria zaliczenia:**

- MapGroup.

- wersjonowany prefiks.

- spójne nazwy.

## Ćwiczenie 5. Mini Notes API in-memory — 30 pkt

Zbuduj API notatek w pamięci: GET all, GET by id, POST, DELETE wraz z poprawnymi kodami HTTP.

**Kryteria zaliczenia:**

- 4 endpointy.

- model/DTO.

- 200/201/404/204.

- walidacja podstawowa.

---

**Suma: 100 pkt**
