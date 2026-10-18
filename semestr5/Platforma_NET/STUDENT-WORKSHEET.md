# Karta obserwacji studenta — semestr 5

| Obszar | Co zaobserwowałem? | Pytanie |
|---|---|---|
| Middleware | | Dlaczego kolejność ma znaczenie? |
| DI | | Co oznacza scoped lifetime? |
| EF Core | | Kiedy LINQ zamienia się w SQL? |
| JWT | | Dlaczego 401 i 403 to nie to samo? |
| Diagnostyka | | Jak znaleźć żądanie, które zakończyło się 500? |
| Deployment | | Dlaczego do uruchomienia API nie potrzeba SDK? |

**Zadanie obserwacyjne:** Wywołaj dwukrotnie `/orders/42`; porównaj `X-Correlation-ID`, `scopeId` i logi. Następnie wywołaj `/fail`. Opisz, co klient wie o błędzie, a co operator widzi w logach.
