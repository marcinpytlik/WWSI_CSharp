# 03 — Errors & Diagnostics
- Wyjątki, wzorzec Result/ProblemDetails (mini), `Activity` (TraceId).


## Zadania domowe
- [ ] Dodaj mapowanie wyjątków domenowych na `Problem` z kodami 4xx.
- [ ] Rozszerz `Problem` o `Errors` (lista błędów walidacyjnych).
- [ ] Dodaj `Activity.Baggage` i sprawdź propagację TraceId w testach.
- [ ] Zrób helper do logowania w formacie: `[{TraceId}] {Message}`.
