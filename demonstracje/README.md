# Demonstracje — 31 mini-projektów

Gotowe, małe aplikacje do pokazania na zajęciach (5–15 min każde).
Każda ma `src` + testy xUnit. Nie zastępują laboratoriów — to **żywe przykłady**
ścieżki semestr 3 → 5.

```bash
dotnet test WWSI_CSharp_NET10.sln --filter "FullyQualifiedName~Demo"
# albo jeden projekt:
dotnet run --project demonstracje/01_KalkulatorCli/src -- add 2 3
```

| # | Projekt | Semestr | Pokazuje |
|---|---|---|---|
| 01 | [Kalkulator CLI](01_KalkulatorCli/) | 3 | metody, wyjątki, kody wyjścia, testy |
| 02 | [Konto bankowe](02_KontoBankowe/) | 3 | hermetyzacja, niezmienniki |
| 03 | [LINQ zamówienia](03_LinqZamowienia/) | 4 | Where/GroupBy/Sum |
| 04 | [CSV → JSON](04_CsvDoJson/) | 4 | IO, `System.Text.Json` |
| 05 | [HTTP cytaty](05_HttpCytaty/) | 4 | `HttpClient`, async, fake handler |
| 06 | [ToDo warstwy](06_TodoWarstwy/) | 4 | serwis + repo JSON |
| 07 | [Notes API](07_NotesApi/) | 5 | Minimal API, CRUD in-memory |
| 08 | [Biblioteka EF](08_BibliotekaEf/) | 5 | EF Core + SQLite |
| 09 | [JWT mini](09_JwtMini/) | 5 | login + chroniony `/me` |
| 10 | [Wycena (strategia)](10_WycenaStrategia/) | 4–5 | Strategy + Factory |
| 11 | [Kolejka Rabbit + SQL Server](11_KolejkaRabbitSql/) | 5 | RabbitMQ, worker, EF Core SQL Server |
| 12 | [EF Core SQL: Code/DB First](12_EfCoreSqlLeastPrivilege/) | 5 | SQL Server, migracje vs scaffold, dwa konta (least privilege) |
| 13 | [Figury (polimorfizm)](13_FiguryPolimorfizm/) | 3 | dziedziczenie, `Shape.Area()` |
| 14 | [Magazyn generyczny](14_MagazynGeneryki/) | 3 | `Repository<T>`, `IEquatable` |
| 15 | [`using` / Dispose](15_UsingDispose/) | 3 | `IDisposable`, wyjątek nadal zamyka zasób |
| 16 | [CSV / XML / JSON](16_CsvXmlJson/) | 4 | jeden model, trzy kodeki |
| 17 | [HTTP + Polly](17_HttpRetryPolly/) | 4 | retry po 500, `CancellationToken` |
| 18 | [Kanał in-process](18_KanalChannel/) | 4 | `Channel<T>` producer/consumer |
| 19 | [Mock zegara i e-maila](19_MockZegarEmail/) | 4 | `IClock`, Moq |
| 20 | [Walidacja Minimal API](20_WalidacjaMinimalApi/) | 5 | DataAnnotations, 400 Problem Details |
| 21 | [Relacje EF 1–n](21_RelacjeEf/) | 5 | `Author`–`Book`, `Include` |
| 22 | [Middleware + logi](22_MiddlewareLogger/) | 5 | correlation id, 500 → problem+json |
| 23 | [Test integracyjny API](23_TestIntegracyjnyApi/) | 5 | `WebApplicationFactory`, izolowany SQLite |
| 24 | [Outbox + kolejka](24_OutboxRabbit/) | 5 | najpierw baza, potem bus; retry po awarii |
| 25 | [Idempotency-Key](25_IdempotencyKey/) | 5 | dwa POST-y, jedno zamówienie |
| 26 | [Stan zamówienia](26_StanZamowienia/) | 4–5 | State: New → Paid → Shipped |
| 27 | [Dekorator ceny](27_DekoratorCeny/) | 4–5 | rabat / VAT / dostawa, kolejność ma znaczenie |
| 28 | [Observer](28_ObserverZdarzenia/) | 4–5 | `event`, odpinanie listenera |
| 29 | [Cache w pamięci](29_CachePamiec/) | 5 | `IMemoryCache` (Redis opcjonalnie) |
| 30 | [Health checks](30_HealthChecks/) | 5 | `/health` 200 vs 503 |
| 31 | [SQL read/write](31_ReadWriteSql/) | 5 | dwa konta: zapis vs tylko SELECT |

Kolejność na zajęciach: **01–02, 13–15** (sem. 3), **03–06, 16–19** (sem. 4), **07–09, 20–23, 25, 30** (sem. 5), **10, 26–28** (wzorce), **11–12, 24, 29, 31** na zajęcia z Dockerem / infrastrukturą.
