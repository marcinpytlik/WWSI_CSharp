# Tydzień 6 – Async/await

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. Opóźnione zadanie — 10 pkt

Napisz metodę async symulującą operację I/O przez Task.Delay i zwracającą wynik.

**Kryteria zaliczenia:**

- async/await.

- Task<T>.

- brak .Result/.Wait.

## Ćwiczenie 2. Równoległe zadania — 15 pkt

Uruchom 5 niezależnych operacji i poczekaj na nie przez Task.WhenAll.

**Kryteria zaliczenia:**

- WhenAll.

- poprawne wyniki.

- pomiar czasu.

## Ćwiczenie 3. Obsługa wyjątku async — 20 pkt

Zademonstruj propagację i obsługę wyjątku z metody asynchronicznej.

**Kryteria zaliczenia:**

- throw w Task.

- await.

- poprawny try/catch.

## Ćwiczenie 4. CancellationToken — 25 pkt

Dodaj możliwość anulowania długiej operacji przez CancellationToken.

**Kryteria zaliczenia:**

- Token w API.

- ThrowIfCancellationRequested lub Delay(token).

- obsługa anulowania.

## Ćwiczenie 5. Asynchroniczny importer — 30 pkt

Przetwórz wiele plików jednocześnie, ograniczając liczbę równoległych operacji i raportując postęp.

**Kryteria zaliczenia:**

- async I/O.

- kontrola współbieżności.

- cancellation.

- raport postępu.

---

**Suma: 100 pkt**
