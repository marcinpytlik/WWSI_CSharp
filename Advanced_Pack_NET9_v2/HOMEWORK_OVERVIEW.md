# Zadania domowe — przegląd (1–8)

## 01_Async_IO
- [ ] Napisz `WithCancellation` dla `IAsyncEnumerable<T>` (przerywa enumerację po czasie).
- [ ] Dodaj `RetryAsync` z wykładniczym backoffem i jitterem.
- [ ] Zaimplementuj `CircuitBreaker` (progi błędów, reset po czasie).
- [ ] Napisz testy z `CancellationTokenSource.CancelAfter`.

## 02_LINQ_Collections
- [ ] Zaimplementuj własny `SelectMany` dla `IEnumerable<T>` (ćwiczenie) i porównaj z LINQ.
- [ ] Porównaj alokacje: `ToList()` vs `Span<T>` w sumowaniu (BenchmarkDotNet).
- [ ] Napisz rozszerzenie `Chunked<T>(int size)` i przetestuj przypadki brzegowe.
- [ ] Przeanalizuj złożoność `OrderBy().ThenBy()` na dużych danych.

## 03_Errors_Diagnostics
- [ ] Dodaj mapowanie wyjątków domenowych na `Problem` z kodami 4xx.
- [ ] Rozszerz `Problem` o `Errors` (lista błędów walidacyjnych).
- [ ] Dodaj `Activity.Baggage` i sprawdź propagację TraceId w testach.
- [ ] Zrób helper do logowania w formacie: `[{TraceId}] {Message}`.

## 04_Testing_Advanced
- [ ] Napisz prosty **assert snapshot** (zapisuje/porównuje plik `.snap`).
- [ ] Dodaj `FakeClock` z ruchem do przodu (`AdvanceBy`).
- [ ] Stwórz testy równoległe dla metody bezpiecznej wątkowo.
- [ ] Przygotuj `DataDriven` test z `Theory` + `InlineData`.

## 05_EFCore9
- [ ] Dodaj relację 1..N (`Product` → `Review`) z ograniczeniami i indeksem.
- [ ] Napisz test transakcji i `AsNoTracking` dla zapytań tylko do odczytu.
- [ ] Dodaj `ValueConverter` dla `Money` (decimal, currency).
- [ ] Stwórz migrację i sprawdź skrypt SQL (na lokalnym projekcie z SQL Server).

## 06_ASPNETCore9_MinimalAPI
- [ ] Dodaj `ProblemDetails` i spójny handler błędów (400/404/500).
- [ ] Zabezpiecz `/api/v1/items` JWT-em i napisz test 401.
- [ ] Wprowadź ograniczanie szybkości (rate limiting) dla POST.
- [ ] Dodaj `/healthz/ready` i `/healthz/live` (liveness/readiness).

## 07_Performance
- [ ] Dodaj benchmark porównujący `string.Concat` vs `StringBuilder`.
- [ ] Zrób benchmark kopii tablicy: `Array.Copy` vs pętla.
- [ ] Zmierz wpływ `Span<T>` w sumowaniu liczb.
- [ ] Wyniki eksportuj do `BenchmarkDotNet.Artifacts` i opisz w README.

## 08_DevOps
- [ ] Zbuduj obraz API, uruchom `docker-compose up`, sprawdź healthchecks.
- [ ] Dodaj krok do GH Actions: publikacja artefaktów testów (TRX).
- [ ] Skonfiguruj **matrix build** (Linux/Windows) w workflow.
- [ ] Rozszerz `docker-compose` o `seq` (logi) i połącz API z logowaniem.
