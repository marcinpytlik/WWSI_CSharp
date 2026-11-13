# 66 — Web API → SQL Outbox → RabbitMQ → Worker → SQL Server + Redis

**WWSI / .NET 10 / EF Core 10 / SQL Server 2022 / RabbitMQ / Redis**

Demonstracja architektury z **eventual consistency**, trwałym SQL Outbox, idempotencją HTTP oraz Workera, ograniczonym retry i Dead Letter Queue, bezpieczeństwem API, minimalnymi uprawnieniami SQL i OpenTelemetry.

> To wariant **production-oriented reference**, nie certyfikat gotowości produkcyjnej. Do wdrożenia wymagane są testy całego środowiska, decyzje HA/DR, rzeczywisty dostawca OIDC, TLS, zarządzanie sekretami oraz operatorzy/alerty (zob. [OPERATIONS.md](OPERATIONS.md)).

## Architektura docelowa

```mermaid
flowchart LR
  C["Klient JWT"] -->|"POST Idempotency-Key"| A["ASP.NET Core API<br/>wwsi_api"]
  C -->|"GET"| A
  A -->|"TX: OperationStatus + Outbox"| DB[("SQL Server 2022")]
  A <-->|"GET/SET TTL 15s"| R[("Redis")]
  A -->|"cache miss: SELECT"| DB
  O["OutboxPublisher<br/>wwsi_worker"] -->|"SELECT / UPDATE outbox"| DB
  O -->|"persistent publish + confirm"| Q[("RabbitMQ products.create")]
  Q --> W["Consumer<br/>wwsi_worker"]
  W -->|"transaction: product + status + dedup"| DB
  W -.->|"DEL after COMMIT"| R
  W -->|"ACK after COMMIT"| Q
  W -->|"transient failure"| Retry[("products.retry<br/>TTL 5 s")]
  Retry -->|"dead-letter to main"| Q
  Q -->|"terminal NACK"| DLQ[("products.dead")]
  I["db-init (manual)<br/>provisioning only"] -.-> DB
```

Każdy produkt posiada `PrincipalId`; SQL odczyt jest filtrowany według właściciela, a klucz Redis obejmuje identyfikator produktu i właściciela.

Kluczowa zmiana wobec pierwszej wersji: **202 Accepted oznacza COMMIT operacji i jej rekordu Outbox w SQL**, a nie potwierdzenie publikacji w RabbitMQ. Relay może opublikować polecenie później. Wiele replik relaya koordynuje pobieranie wiadomości przez odnawialny 60-sekundowy lease; po awarii i wygaśnięciu dzierżawy publikacja może się powtórzyć. Komunikat może przyjść więcej niż raz; Worker zapisuje `ProcessedCommands` w tej samej transakcji co produkt i status `Completed`.

```mermaid
sequenceDiagram
  participant U as Klient
  participant A as API
  participant D as SQL Server
  participant P as Outbox Publisher
  participant Q as RabbitMQ
  participant W as Worker
  participant R as Redis
  U->>A: POST /products + Bearer + Idempotency-Key
  A->>D: BEGIN; OperationStatus Pending + Outbox; COMMIT
  A-->>U: 202 Accepted (operationId, productId)
  P->>D: SELECT unpublished Outbox
  P->>Q: Publish persistent / confirm
  P->>D: Mark PublishedAtUtc
  Q->>W: Deliver
  W->>D: BEGIN; dedup + INSERT Product + Completed; COMMIT
  W->>R: DEL products:{id}
  W-->>Q: ACK
  U->>A: GET /operations/{id}
  A->>D: SELECT own operation
  A-->>U: Completed
  U->>A: GET /products/{id}
  A->>R: GET key
  alt MISS
    A->>D: SELECT product
    A->>R: SET with TTL
  end
  A-->>U: 200 + X-Cache MISS/HIT
```

### Struktura

```text
66_WebApiRabbitRedisSql/
├── src/
│   ├── Api/
│   │   ├── Middleware/ (request logging / centralized exceptions)
│   │   └── Program.cs (JWT, rate limit, endpoints, SQL Outbox)
│   ├── Worker/
│   │   └── Program.cs (OutboxPublisher + Consumer + bounded retry + metrics)
│   └── Shared/
│       ├── Contracts/
│       ├── Entities/ (Product, ProcessedCommand, OperationStatus, OutboxMessage)
│       └── Data/
│           ├── AppDbContext.cs
│           └── Configuration/ (IEntityTypeConfiguration<T>)
├── sql/
│   ├── 01-bootstrap.sql (provisioning, grants, schema)
│   └── 02-verify-permissions.sql
├── scripts/
│   ├── New-DemoJwt.ps1
│   └── Smoke-Test.ps1
├── tests/QueueCacheDemo.Tests/
├── .env.example
└── docker-compose.yml
```

## Minimalne uprawnienia SQL

`wwsi_api`: SELECT Products, SELECT/INSERT OperationStatuses, INSERT OutboxMessages. **Brak INSERT Products, UPDATE Outbox, DDL.**

`wwsi_worker`: SELECT/INSERT Products i ProcessedCommands, SELECT/UPDATE OperationStatuses i OutboxMessages. **Brak DDL i szerokich ról db_datawriter/db_owner.**

`sa` używane **tylko podczas świadomie uruchamianego provisioningu**; API i Worker nie mają jego hasła. Skrypt bootstrapowania nie zmienia haseł istniejących loginów. Przy istniejącym wolumenie zachowaj zgodność `.env` z hasłami kont SQL, albo przeprowadź osobną, kontrolowaną rotację.

## Pierwszy start / aktualizacja istniejącego laboratorium

Z poziomu `demonstracje/66_WebApiRabbitRedisSql`:

```powershell
git pull
if (-not (Test-Path .env)) { Copy-Item .env.example .env }
# Sprawdź wartości w .env. W istniejącym wolumenie pozostaw aktualne hasło sa.
docker compose config --quiet
docker compose up -d sql rabbitmq redis
docker compose --profile setup run --rm db-init
docker compose up -d --build api worker
docker compose ps
```

**Nie używaj `docker compose down -v`** — to usunie wolumeny SQL Server i RabbitMQ. RabbitMQ otrzymał w tej wersji nowy trwały wolumen `rabbitmq_data` oraz stałą nazwę węzła; **pierwsze przejście ze starego kontenera bez wolumenu wymaga sprawdzenia/eksportu zaległych wiadomości i DLQ przed jego odtworzeniem**. Stare wiadomości (z pierwszej wersji bez SQL Outbox) nie mają automatycznej drogi odtworzenia z SQL. Dane istniejące w SQL pozostają na wolumenie `sql_data`. `docker compose config` bez `--quiet` wypisuje sekrety, dlatego nie publikuj pełnego wyniku.

Dostęp lokalny: API `http://localhost:8080`, RabbitMQ UI `http://localhost:15672`, SQL `localhost,14353`. Redis jest dostępny tylko wewnątrz sieci Compose.

### Test end-to-end

```powershell
.\scripts\Smoke-Test.ps1
```

Skrypt tworzy **lokalny token HS256 wyłącznie na potrzeby laboratorium**, wysyła POST z `Idempotency-Key`, powtarza POST, weryfikuje ten sam `operationId`, oczekuje `Completed` i sprawdza `MISS → HIT`.

Ręcznie:

```powershell
$token = .\scripts\New-DemoJwt.ps1
$headers = @{ Authorization = "Bearer $token"; "Idempotency-Key" = [guid]::NewGuid().ToString() }
$body = @{ name = "WWSI"; price = 99.90 } | ConvertTo-Json
$r = Invoke-RestMethod http://localhost:8080/products -Method Post -Headers $headers -ContentType application/json -Body $body
Invoke-RestMethod "http://localhost:8080/operations/$($r.operationId)" -Headers $headers
Invoke-WebRequest "http://localhost:8080/products/$($r.productId)" -Headers $headers
```

`GET /health/live` nie sprawdza zależności. `GET /health/ready` wykonuje rzeczywisty test SQL, Redis i połączenia RabbitMQ.

## Mechanizmy i gwarancje

| Obszar | Realizacja | Ograniczenie |
|---|---|---|
| Idempotencja HTTP | unikalne (PrincipalId, IdempotencyKey) + hash treści | każdy nowy klucz to osobne polecenie |
| Outbox | API zapisuje Pending + Outbox w jednej transakcji | relay posiada lease z wygaśnięciem, ale nadal możliwy jest duplikat po awarii |
| Worker | at-least-once; Serializable; ProcessedCommands PK | przejściowy błąd COMMIT wymaga testu awaryjnego |
| Retry | do 3 ponowień przez products.retry (TTL 5s) | po wyczerpaniu terminalne NACK → DLQ; brak automatycznego replay |
| Status | Pending, Completed, Failed | Failed może pozostać Pending, gdy SQL nie działa i nie da się zapisać błędu |
| Redis | cache-aside + TTL 15s, invalidation po COMMIT | brak silnej spójności; race między odczytem i invalidacją |
| Auth | JWT Bearer z walidacją podpisu, issuer, audience, exp | lokalny HS256 token tool; produkcyjnie wymagany OIDC/JWKS |
| Rate limiting | limit 60 żądań/min na tożsamość/IP | dla wielu replik wymaga limitera współdzielonego / gateway |
| Telemetry | OpenTelemetry API (ASP.NET) i Worker (Activity + Meter) | exporter wymaga skonfigurowanego kolektora OTLP |
| Secrets | lokalny `.env`, ignorowany przez Git | produkcyjnie wymagany secret manager, rotacja, TLS |

Konfiguracja OTLP: opcjonalnie ustaw w `.env` `OTEL_EXPORTER_OTLP_ENDPOINT=http://collector:4317` i zapewnij dostępny kolektor. Brak kolektora oznacza brak zdalnego eksportu, ale lokalne logi nadal działają.

## Testy, CI/CD i eksploatacja

```powershell
dotnet test .\tests\QueueCacheDemo.Tests\QueueCacheDemo.Tests.csproj
docker compose logs --tail=100 api worker
docker compose exec rabbitmq rabbitmqctl list_queues name messages_ready messages_unacknowledged consumers
```

Patrz: [OPERATIONS.md](OPERATIONS.md) — scenariusze awarii, backup/restore, wdrożenie, zagrożenia, wymagane testy i decyzje HA/DR. CI kompiluje projekty i uruchamia testy; pełny smoke test Dockera jest dostępny jako osobny przebieg manualny.

### Stan weryfikacji

Poprzednia wersja (11.11.2026) przeszła test `accepted → completed → MISS → HIT`. **Obecna refaktoryzacja wymaga powtórzenia build i smoke-testu**; nie należy interpretować wyniku poprzedniej wersji jako potwierdzenia tej implementacji.


## Error Handling & Resilience

### Centralna obsługa błędów API

`Api/ErrorHandling/GlobalExceptionHandler.cs` implementuje `IExceptionHandler` i jest rejestrowany przez
`AddExceptionHandler<GlobalExceptionHandler>()`, `AddProblemDetails()` oraz `UseExceptionHandler()`.
Nie stosujemy już ręcznie pisanego middleware `ExceptionHandlingMiddleware`.

| Źródło | Odpowiedź | Kod stabilny |
|---|---|---|
| RequestValidationException | HTTP 400 | VALIDATION_ERROR |
| ResourceNotFoundException | HTTP 404 | NOT_FOUND |
| RequestConflictException | HTTP 409 | CONFLICT |
| DependencyUnavailableException / znany przejściowy wyjątek SQL / timeout | HTTP 503 + Retry-After: 5 | DEPENDENCY_UNAVAILABLE |
| Nieznany wyjątek | HTTP 500 | UNEXPECTED_ERROR |
| Brak uwierzytelnienia / uprawnień | HTTP 401/403 z middleware autoryzacji | — |

Odpowiedzi błędów domenowych mają `application/problem+json`, `code` i `traceId`. Serwer rejestruje nieobsłużone wyjątki w logach; klient nie otrzymuje stack trace ani connection string. Przerwane przez klienta żądania są wyłączone z prób pisania odpowiedzi błędu.

### Worker — klasyfikacja zamiast ślepego retry

`Shared/Errors/FailureClassifier.cs` zwraca `Permanent` lub `Transient`:

- **Permanent:** niepoprawny JSON/kontrakt polecenia, błędny identyfikator właściciela, trwale niepoprawne dane; komunikat trafia bez zwłoki do DLQ.
- **Transient:** znane błędy przejściowe SQL (timeout, deadlock, failover), I/O i niedostępność zależności, oraz nieznane wyjątki (ostrożnie: maks. 3 ponowienia). Retry używa kolejki `products.retry` z TTL 5 s.
- **Terminal:** po wyczerpaniu liczby ponowień operacja jest oznaczana `Failed`, jeżeli SQL jest osiągalny, i wiadomość trafia do `products.dead`. Jeśli SQL jest niedostępny, status może zostać `Pending` — wymaga to alertu na wiek operacji.
- **COMMIT / ACK:** po zatwierdzeniu SQL deduplikacja oparta na `ProcessedCommands` zapobiega ponownemu INSERT w przypadku ponownej dostawy.

`ApplicationExceptionBase` i typy wyjątków są w projekcie `Shared/Errors`, dzięki czemu ten sam model błędów jest dostępny dla API i Workera.

### Weryfikacja

```powershell
dotnet test .\tests\QueueCacheDemo.Tests\QueueCacheDemo.Tests.csproj
docker compose up -d --build api worker
.\scripts\Smoke-Test.ps1
```

Smoke test sprawdza dodatkowo odpowiedzi **400, 409, 404 i 401**. Testy xUnit weryfikują klasyfikację błędnych poleceń i timeoutów. Zachowanie w trakcie awarii SQL i brokera wymaga osobnych scenariuszy awaryjnych.

## Timeout Policy & Dependency Resilience

Nowa polityka API (ustawienia z `docker-compose.yml`):

| Ustawienie | Wartość laboratorium |
|---|---|
| Resilience__SqlConnectTimeoutSeconds | 3 |
| Resilience__SqlCommandTimeoutSeconds | 10 |
| Resilience__SqlRetryCount | 1 |
| Resilience__ReadinessTimeoutSeconds | 5 |

`/health/ready` sprawdza SQL bez retry EF Core przy użyciu osobnego połączenia,
a jego termin upływa po 5 sekundach (zależnie od poprawnej obsługi anulowania
przez sterowniki zależności). Zwrot błędu zależności to HTTP 503.
`/health/live` działa niezależnie od SQL. POST nadal zapisuje
`OperationStatuses` i `OutboxMessages` atomowo. Nie stosujemy
agresywnego anulowania w środku SQL COMMIT: niepewny wynik należy ponowić
z **tym samym** `Idempotency-Key` i payloadem.

Laboratoryjny test awarii (czasowo zatrzymuje kontener SQL):
```powershell
.\scripts\Test-SqlOutage.ps1
```
Skrypt uruchamia SQL ponownie w bloku finally. Nie usuwa wolumenów.
Uruchamiaj wyłącznie na dedykowanym środowisku demonstracyjnym.

## Eksperyment 5: retry wiadomości RabbitMQ (błąd przejściowy)

Na **wydzielonym laboratorium** `Test-Retry.ps1` włącza
tymczasowo `Chaos__EnableRetryProbe=true` wyłącznie dla Workera.
Gdy nazwa produktu zaczyna się od `RetryProbe-`, Worker zgłasza
kontrolowany `TimeoutException` tylko podczas pierwszej dostawy
(`x-demo-attempt=0`). SQL Outbox pozostaje bez zmian.

Worker publikuje wiadomość na `products.retry`, gdzie TTL = 5 sekund,
a następnie RabbitMQ przekierowuje ją do `products.create` z
nagłówkiem `x-demo-attempt=1`. Ponowione przetwarzanie powinno zakończyć się
stanem `Completed`. Skrypt sprawdza status i logi:
wstrzyknięcie błędu, zaplanowany retry oraz ukończenie operacji.

```powershell
dotnet test .\tests\QueueCacheDemo.Tests\QueueCacheDemo.Tests.csproj
docker compose up -d --build api worker
.\scripts\Test-Retry.ps1
.\scripts\Smoke-Test.ps1
```

Skrypt przywraca poprzednie ustawienie zmiennej środowiskowej i tworzy
kontener Worker ponownie w bloku `finally`, nawet gdy test się nie powiedzie.
Bez `CHAOS_ENABLE_RETRY_PROBE=true` celowe błędy są **wyłączone**.
Nie używaj tego testu na środowisku współdzielonym lub produkcyjnym.

## Eksperyment 6: Dead Letter Queue — dwa scenariusze

Uruchom tylko na **wydzielonym laboratorium**:

```powershell
git pull
dotnet test .\tests\QueueCacheDemo.Tests\QueueCacheDemo.Tests.csproj
docker compose up -d --build api worker
.\scripts\Test-Dlq.ps1
.\scripts\Smoke-Test.ps1
```


`Test-Dlq.ps1` sprawdza dwa przypadki:

1. Niepoprawny JSON publikuje bezpośrednio do `products.create`
   przez RabbitMQ Management HTTP API, po czym weryfikuje wzrost
   liczby wiadomości w `products.dead`. Błąd jest trwały
   i **nie powinien** przechodzić retry.
2. Poprawny POST używa prefiksu `DeadLetterProbe-`, który przy
   `Chaos__EnableDlqProbe=true` włącza kontrolowany błąd przejściowy
   przy **każdej** dostawie Workera. Oczekujemy 3 publikacji do
   `products.retry` (TTL 5 s), czterech dostaw,
   statusu `Failed` z kodem `RETRIES_EXHAUSTED`
   oraz drugiej wiadomości w `products.dead`.

Skrypt czyta konto RabbitMQ z lokalnego `.env`
(bez wypisywania hasła), wykonuje asercje na statusach/logach
i liczniku DLQ, a następnie odtwarza konfigurację Workera w `finally`.
Nie usuwa wiadomości z DLQ ani wolumenów, zatem kolejne uruchomienia
zwiększają rozmiar `products.dead`. Zakładamy środowisko
laboratoryjne bez innych równoległych producentów wiadomości.

## Eksperyment 7: Idempotent Consumer — wielokrotne dostarczenie

Po zakończeniu oryginalnego POST i COMMIT w SQL Server uruchamiamy
`Test-IdempotentConsumer.ps1` na wydzielonym środowisku demo.
Skrypt odczytuje z `dbo.OutboxMessages` dokładny, pierwotny
`Payload` (SQL `sa` działa wyłącznie wewnątrz kontenera)
i publikuje go **dwa razy** przez RabbitMQ Management API do
`products.create`. Nie symuluje żądania HTTP —
testuje wielokrotną **dostawę do konsumenta**.

Asercje po każdej dostawie: dla wskazanych GUID istnieje dokładnie:
jeden `dbo.Products`, jeden `dbo.ProcessedCommands`
i jedna operacja `dbo.OperationStatuses` w stanie
`Completed`. Weryfikujemy też odnotowane przetworzenia
wiadomości w logach Workera. Ten eksperyment weryfikuje deduplikację
**po COMMIT**, nie testuje jeszcze awarii dokładnie między COMMIT i ACK.

```powershell
git pull
docker compose up -d --build api worker
.\scripts\Test-IdempotentConsumer.ps1
.\scripts\Smoke-Test.ps1
```

Skrypt nie restartuje usług, nie usuwa komunikatów ani wolumenów.
Wymaga lokalnego RabbitMQ Management na porcie 15672 i kontenera SQL
z `sqlcmd`. Korzystaj wyłącznie z wydzielonego
środowiska laboratoryjnego.
