# Tydzień 8 – Middleware i logowanie

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. Request timing — 10 pkt

Napisz middleware mierzący czas żądania i logujący metodę, ścieżkę oraz czas.

**Kryteria zaliczenia:**

- Custom middleware.

- stopwatch.

- jedno logowanie na request.

## Ćwiczenie 2. Correlation ID — 15 pkt

Dodaj correlation id do każdego żądania i odpowiedzi.

**Kryteria zaliczenia:**

- Nagłówek.

- generowanie gdy brak.

- użycie w logach.

## Ćwiczenie 3. Global exception handling — 20 pkt

Dodaj centralną obsługę wyjątków zwracającą ProblemDetails bez stack trace dla klienta.

**Kryteria zaliczenia:**

- Global handler.

- ProblemDetails.

- 500.

- brak wycieku stack trace.

## Ćwiczenie 4. Structured logging — 25 pkt

Skonfiguruj Serilog/ILogger do logów strukturalnych i dodaj pola UserId/CorrelationId tam, gdzie są dostępne.

**Kryteria zaliczenia:**

- Structured properties.

- levels.

- brak sekretów/PII w logach.

## Ćwiczenie 5. Observability mini-stack — 30 pkt

Dodaj health check zależności, request logging, correlation id i centralne błędy. Opisz w README jak diagnozować jedno przykładowe żądanie od wejścia do błędu.

**Kryteria zaliczenia:**

- 4 elementy observability.

- spójne logi.

- health.

- instrukcja diagnostyczna.

---

**Suma: 100 pkt**
