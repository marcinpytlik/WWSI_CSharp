# 01 — Async & I/O
- `async/await`, `CancellationToken`, timeouts, `IAsyncEnumerable`, prosty retry/backoff.


## Zadania domowe
- [ ] Napisz `WithCancellation` dla `IAsyncEnumerable<T>` (przerywa enumerację po czasie).
- [ ] Dodaj `RetryAsync` z wykładniczym backoffem i jitterem.
- [ ] Zaimplementuj `CircuitBreaker` (progi błędów, reset po czasie).
- [ ] Napisz testy z `CancellationTokenSource.CancelAfter`.
