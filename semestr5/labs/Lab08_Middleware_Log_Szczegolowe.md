# Lab08 Middleware i logowanie — szczegółowe laboratorium

**Czas:** 90–120 min

## Cel

Uporządkować pipeline i logować żądania **bez** wycieku sekretów.

## Zadania

1. `UseExceptionHandler` albo middleware łapiący wyjątki → 500 ProblemDetails (w Development można więcej szczegółów).
2. Middleware correlation: nagłówek `X-Correlation-Id` (przyjmij albo wygeneruj GUID).
3. `ILogger` na request path + status code.
4. Nie loguj `Authorization` ani body z hasłem.

## Kryteria

- [ ] wyjątek w endpointcie nie wywala procesu
- [ ] correlation id wraca w odpowiedzi
- [ ] test: rzuć wyjątek z testowego `/boom` → 500
