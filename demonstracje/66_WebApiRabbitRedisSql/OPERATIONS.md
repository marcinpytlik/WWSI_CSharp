# Demo 66 — runbook operacyjny i droga do produkcji

## Zasady eksploatacji

1. Najpierw backup SQL (FULL, LOG według RPO) i sprawdzenie możliwości odtworzenia; potem migracja schematu, na końcu wdrożenie API/Worker.
2. `db-init` uruchamiaj świadomie jako krok deploymentu, nigdy automatycznie przy restarcie API. W produkcji zastąp go wersjonowanymi migracjami z zatwierdzeniem DBA.
3. Sekrety nie mogą trafić do repo ani logów. Laboratorium korzysta z `.env`; produkcja wymaga secret manager, TLS i osobnych kont serwisowych.
4. Idempotency-Key jest obowiązkowy; klient ponawia ten sam klucz **tylko** dla tego samego żądania.
5. Status `Pending` może oznaczać również problem z relayem. Monitoruj wiek Outbox, backlog w RabbitMQ i DLQ, nie tylko HTTP 200.

## Scenariusze kontrolowane

| Próba | Sposób | Kryterium |
|---|---|---|
| Deduplikacja POST | Dwa POST z tym samym kluczem i treścią | ten sam operationId i productId |
| Konflikt klucza | Ten sam klucz i zmieniony payload | HTTP 409 |
| Eventual consistency | Zatrzymaj Worker, wyślij POST, wznow Worker | Pending → Completed, bez drugiego produktu |
| Broker offline | Zatrzymaj RabbitMQ, wykonaj POST | 202 po COMMIT w SQL; relay publikuje po wznowieniu |
| Redis offline | Zatrzymaj Redis, wykonaj GET | fallback SQL, ponowne cache po powrocie Redis |
| SQL offline | Zatrzymaj SQL, wykonaj POST/GET | błąd zależności bez utraty już zatwierdzonych wiadomości |
| Worker po COMMIT przed ACK | Kill Workera w kontrolowanym punkcie | redelivery bez drugiego INSERT |
| DLQ | Symuluj nieskuteczne polecenie | Retry ograniczony, następnie products.dead |
| SQL permissions | Wykonaj 02-verify-permissions.sql | API bez INSERT Products, Worker bez ALTER |
| Odczyty cache | GET po Completed dwukrotnie | X-Cache MISS → HIT (w TTL) |

Do testu restartów/awarii użyj kopii bazy laboratoryjnej. Nie modyfikuj kolejki ani produkcyjnych danych bez procedury.

## Monitoring i alerty — wymagane przed produkcją

- API: request count, error rate, latency p50/p95/p99, HTTP 429, status readiness
- SQL: czas komitów, blokady/deadlocki, Query Store, błędy 18456, wzrost logu, backup chain
- RabbitMQ: ready/unacked, oldest message age, reconnects, publisher confirms, DLQ depth, disk/memory alarms
- Outbox: `SELECT COUNT(*), MIN(CreatedAtUtc) FROM dbo.OutboxMessages WHERE PublishedAtUtc IS NULL`
- Status: liczba `Pending` starszych niż ustalony SLO i `Failed`
- Redis: hit/miss, p95 latency, memory, eviction, timeout, fallback SQL rate
- Worker: `wwsi.commands.completed`, `wwsi.commands.retried`, `wwsi.commands.deadlettered`

Właściwe progi alarmowe, SLO i dashboardy muszą wynikać z testów obciążeniowych i ustaleń operacyjnych. `/health/ready` w API sprawdza SQL, Redis i RabbitMQ.

## Wariant wysokiej dostępności

- API: co najmniej dwie repliki za gateway z TLS, OIDC, rate limit współdzielonym i propagacją traceparent.
- RabbitMQ: quorum queues na osobnych hostach/AZ, polityka DLQ at-least-once i trwały storage; nie mylić pojedynczego kontenera z HA.
- SQL Server: Always On Availability Groups lub istniejąca architektura FCI zgodnie z RPO/RTO; test failover i obsługi niejednoznacznego COMMIT.
- Redis: Sentinel/Cluster lub usługa zarządzana; test failover, pamięci i wygaśnięć kluczy.
- Worker: wiele replik bez utraty idempotencji. Outbox relay stosuje krótką dzierżawę/claim (60 s). Po wygaśnięciu lease wiadomość może zostać opublikowana ponownie, co jest obsługiwane idempotentnie w SQL. Przed skalowaniem trzeba przetestować konkurencyjne relay, wygaśnięcie lease, zegary hostów i dostępność RabbitMQ.

## Recovery / rollback

- Odtwarzaj SQL zgodnie z uzgodnionym RPO/RTO. Przywróć także zgodność schematu i kont aplikacyjnych.
- Przy awarii Redis nie odtwarzaj go jako źródła prawdy: po naprawie pozwól cache odbudować się z SQL.
- Po awarii RabbitMQ upewnij się, że relay wznowił publikację z Outbox; porównaj Pending, Outbox i kolejki.
- Replay DLQ: tylko po analizie błędu, zachowując `OperationId`; ręczna procedura pod nadzorem operatora (w tej wersji brak automatycznego endpointu replay).
- Rollback kodu aplikacji wymaga kompatybilnego schematu. Zmiany schematu projektuj expand → migrate → contract, nie usuwaj kolumn w tym samym wdrożeniu.

## Granice obecnego wdrożenia

**Nie ma jeszcze potwierdzonego**: produkcyjnego klastra HA, OIDC/JWKS, TLS na ingress/broker, centralnego secret managera, infrastruktury kolektora/dashboardów i alertów, skanowania obrazów, testu failover ani zmierzonego RPO/RTO i p99. Te elementy wymagają decyzji środowiskowych i realnych testów. `Docker Compose` służy wyłącznie do lokalnego laboratorium.

## Migracja z poprzedniej wersji demonstracji

- Wcześniejszy kontener RabbitMQ nie miał nazwanego wolumenu. Przed odtworzeniem kontenera sprawdź `products.create`, `products.retry` i `products.dead`; w razie potrzeby wyeksportuj wiadomości lub rozpatrz je ręcznie. Nowa wersja ma `rabbitmq_data` i stały nodename.
- Skrypt SQL dodaje `PrincipalId` do produktów, tworzy nowe tabele Outbox i OperationStatuses, zapisuje wersję schematu oraz odbiera API wcześniejszy nadmiarowy SELECT z ProcessedCommands. Istniejące produkty otrzymują `PrincipalId='legacy'` i nie będą widoczne dla innych tokenów.
- Nie wykonuj automatycznego rollbacku schematu w celu powrotu do starej wersji Workera. Najpierw zrób backup oraz sprawdź kompatybilność wiadomości w kolejce.
