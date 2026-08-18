# Demonstracje — 65 mini-projektów

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
| 32 | [API + Hangfire + SQL](32_HangfireApiSql/) | 5 | trzy obrazy Docker: API, worker Hangfire, SQL Server |
| 33 | [JWT role 403](33_JwtRole403/) | 5 | `ClaimTypes.Role`, User vs Admin, 401/403 |
| 34 | [Paginacja i filtr](34_PaginacjaFiltr/) | 5 | `skip`/`take`/`q`, `total` w odpowiedzi |
| 35 | [Relacje n–n](35_RelacjeNn/) | 5 | `Student`–`Course`, `Include` |
| 36 | [Optimistic concurrency](36_OptimisticConcurrency/) | 5 | token wersji, drugi PUT → 409 |
| 37 | [Soft delete](37_SoftDelete/) | 5 | `IsDeleted` + `HasQueryFilter` |
| 38 | [FluentValidation](38_FluentValidation/) | 5 | ten sam produkt co 20, inny walidator |
| 39 | [Redis + SQL](39_RedisCacheSql/) | 5 | trzy obrazy: API, Redis, SQL Server |
| 40 | [MinIO upload + SQL](40_MinioUploadSql/) | 5 | trzy obrazy: API, MinIO, SQL Server |
| 41 | [YARP gateway](41_YarpGateway/) | 5 | trzy obrazy: gateway, API, SQL Server |
| 42 | [Seq + SQL](42_SeqLogging/) | 5 | trzy obrazy: API, Seq, SQL Server |
| 43 | [SignalR + SQL](43_SignalRApiSql/) | 5 | trzy obrazy: API, nginx, SQL Server |
| 44 | [Result](44_ResultType/) | 4–5 | `Ok`/`Fail` zamiast wyjątku na 400 |
| 45 | [CQRS lite](45_CqrsLite/) | 4–5 | osobny command i query handler |
| 46 | [Adapter XML](46_AdapterXml/) | 4–5 | legacy XML za `IQuoteClient` |
| 47 | [Chain of Responsibility](47_ChainOfResponsibility/) | 4–5 | puste → format → limit |
| 48 | [Central Package Management](48_CpmPakiety/) | 4–5 | jedna wersja pakietu, dwa projekty bez `Version=` |
| 49 | [`Directory.Build.props` + SDK](49_DirectoryBuildProps/) | 4–5 | zagnieżdżone propsy, `global.json` |
| 50 | [Options pattern](50_OptionsPattern/) | 5 | `IOptions`, `ValidateOnStart` |
| 51 | [`IHttpClientFactory`](51_HttpClientFactory/) | 4–5 | named client, BaseAddress, timeout |
| 52 | [`TimeProvider`](52_TimeProvider/) | 4 | `FakeTimeProvider` bez Moq |
| 53 | [Wersjonowanie API](53_WersjonowanieApi/) | 5 | `/api/v1` vs `/api/v2` |
| 54 | [Rate limiting](54_RateLimiting/) | 5 | fixed window, 429 |
| 55 | [Output cache](55_OutputCache/) | 5 | drugi GET bez ponownego liczenia |
| 56 | [OpenAPI / Swagger](56_OpenApiSwagger/) | 5 | `/openapi/v1.json` + Swagger UI |
| 57 | [Transakcja EF](57_EfTransakcja/) | 5 | rollback po wyjątku |
| 58 | [`AsNoTracking`](58_AsNoTracking/) | 5 | tracking vs no-tracking przy SaveChanges |
| 59 | [OpenTelemetry + SQL](59_OpenTelemetrySql/) | 5 | trzy obrazy: API, Jaeger, SQL Server |
| 60 | [gRPC + SQL](60_GrpcSql/) | 5 | trzy obrazy: gRPC, HTTP API, SQL Server |
| 61 | [EF + Postgres](61_PostgresEf/) | 5 | trzy obrazy: API, Postgres, Adminer |
| 62 | [Mediator lite](62_MediatorLite/) | 4–5 | `IMediator.Send` (krok po 45) |
| 63 | [Specification](63_Specification/) | 4–5 | `ISpecification<T>` na liście zamówień |
| 64 | [Template Method](64_TemplateMethod/) | 4–5 | import CSV/JSON, wspólna walidacja |
| 65 | [Null Object](65_NullObject/) | 3–4 | `NoDiscount` zamiast `null` |

Kolejność na zajęciach: **01–02, 13–15, 65** (sem. 3), **03–06, 16–19, 48–49, 51–52** (sem. 4), **07–09, 20–23, 25, 30, 33–38, 50, 53–58** (sem. 5), **10, 26–28, 44–47, 62–64** (wzorce), **11–12, 24, 29, 31–32, 39–43, 59–61** na zajęcia z Dockerem / infrastrukturą.
