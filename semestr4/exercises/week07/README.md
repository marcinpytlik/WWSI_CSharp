# Tydzień 7 – HttpClient

**Maksymalna liczba punktów: 100 pkt**

Ćwiczenia wykonuj w .NET 10. Tam, gdzie ma to sens, rozdziel logikę od wejścia/wyjścia i dbaj o czytelne nazwy.

## Ćwiczenie 1. GET JSON — 10 pkt

Pobierz dane z publicznego/testowego endpointu i zdeserializuj JSON.

**Kryteria zaliczenia:**

- HttpClient.

- status code.

- deserialize.

## Ćwiczenie 2. Parametry zapytania — 15 pkt

Zbuduj żądanie GET z parametrami query string tworzonymi na podstawie danych wejściowych.

**Kryteria zaliczenia:**

- Poprawny URI.

- encoding.

- odpowiedź.

## Ćwiczenie 3. Timeout i cancellation — 20 pkt

Skonfiguruj timeout oraz CancellationToken dla żądania HTTP i obsłuż oba przypadki.

**Kryteria zaliczenia:**

- Timeout.

- token.

- rozróżnienie błędów.

## Ćwiczenie 4. Client service — 25 pkt

Ukryj HttpClient za klasą ApiClient z metodami GetByIdAsync i GetAllAsync.

**Kryteria zaliczenia:**

- Separacja odpowiedzialności.

- async.

- obsługa statusów.

## Ćwiczenie 5. Retry z backoff — 30 pkt

Dodaj kontrolowane ponawianie dla błędów przejściowych (bez nieskończonej pętli), z opóźnieniem rosnącym między próbami.

**Kryteria zaliczenia:**

- Limit prób.

- backoff.

- tylko błędy przejściowe.

- log prób.

---

**Suma: 100 pkt**
