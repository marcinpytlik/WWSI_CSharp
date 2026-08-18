# Zadanie 03 – HttpClient z handlerem testowym

**Termin:** tydzień 7

## Treść

Serwis pobierający listę JSON z HTTP. W testach `HttpMessageHandler` / analog jak w `labs/tests/Task10_FetchApiAsync.Tests`.
Timeout i 500 → wyjątek opakowany.

## Kryteria

- [ ] zero sieci w testach
- [ ] `HttpClient` wstrzyknięty (nie `new HttpClient()` w każdej metodzie)
