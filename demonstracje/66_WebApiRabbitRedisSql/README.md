# 66 — Web API → RabbitMQ → SQL Server + Redis (Cache-Aside)

Demonstracja .NET 10: **asynchroniczne zapisy (eventual consistency)** oraz **odczyty z Redis Cache**. SQL Server to system of record. API nie zapisuje bezpośrednio do SQL.

## Architektura

```text
              POST /products
Client ────────> API ──publish──> RabbitMQ ──consume──> Worker ──COMMIT──> SQL Server
  |               |                               |                    |
  | GET /products  |                               +──after commit─────> Redis DEL
  +───────────────> API ──GET key──> Redis
                        └──cache miss──> SQL Server ──SET TTL──> Redis
```

## Start

Wymagane: Docker Desktop z >= 4 GB RAM dostępnej dla SQL Server i Docker Compose.

1. Z katalogu tej demonstracji: `docker compose up --build -d`
2. Stan kontenerów: `docker compose ps`, logi: `docker compose logs -f api worker`
3. Swagger nie jest wymagany; endpoint health: `http://localhost:8080/health`.
4. RabbitMQ UI: `http://localhost:15672` (demo/demo).
5. SQL Server: localhost,14353, login sa, hasło **wyłącznie laboratoryjne** z compose.

### Windows PowerShell

```powershell
$body = @{ name = "Monitor"; price = 799.99 } | ConvertTo-Json
$r = Invoke-RestMethod -Method Post -Uri http://localhost:8080/products -ContentType application/json -Body $body
$r                       # 202 Accepted; operationId oraz productId
Invoke-RestMethod "http://localhost:8080/operations/$($r.operationId)"
Invoke-RestMethod "http://localhost:8080/products/$($r.productId)"
```

Przy pierwszym GET produkt może zwrócić 404, dopóki Worker nie zakończy transakcji: to zamierzone **eventual consistency**. Powtórz GET i porównaj nagłówek `X-Cache` (MISS/HIT). W API GET działa cache-aside z TTL 15 sekund. Brak rekordu nie jest cache'owany.

### Scenariusze badawcze

1. Wykonaj POST, następnie od razu GET oraz GET po przetworzeniu; sprawdź `202` i status operacji.
2. Powtórz GET: pierwszy powinien zwrócić MISS, kolejny HIT.
3. Zatrzymaj Worker (`docker compose stop worker`), wyślij kilka POST, sprawdź gotowe komunikaty w RabbitMQ, uruchom Worker i odczytaj dane.
4. Zatrzymaj Redis; GET korzysta awaryjnie z SQL Server. Przywróć Redis.
5. Sprawdź w SQL `dbo.Products` i `dbo.ProcessedCommands`, obserwując idempotentne przetwarzanie.

### Semantyka i ograniczenia

- API zwraca 202 dopiero po publisher confirm RabbitMQ; nie oznacza to zatwierdzenia rekordu w SQL.
- Worker używa manual ACK **po COMMIT**. Identyfikator operacji jest kluczem w tabeli `ProcessedCommands`; ponowna dostawa komunikatu nie tworzy drugiego produktu.
- W razie błędu przetwarzania komunikat jest odrzucany do DLQ `products.dead` (bez automatycznych retry w tym laboratorium). Operacja może pozostać niepotwierdzona, dopóki nie zostanie obsłużona ręcznie.
- Klucze Redis są usuwane po COMMIT. Inwalidacja nie jest transakcyjna z SQL; przy jej niepowodzeniu TTL ogranicza okres nieaktualności. Możliwy jest również wyścig między cache miss i zapisem. To **nie** zapewnia silnej spójności ani gwarantowanego read-your-writes.
- `GET /operations/{id}` zwraca `completed` lub `pending_or_unknown`; brak osobnej trwałej tabeli statusów poleceń oczekujących / odrzuconych.
- `EnsureCreated` zastosowano wyłącznie do demonstracji (nie do migracji produkcyjnych). Produkcyjnie wymagane: migracje, osobne konta SQL, TLS, sekrety, RBAC, health checks zależności, retry policy + DLQ/replay, obserwowalność i odporna invalidacja.
- Hasła w `docker-compose.yml` są przykładowe: **nie wdrażaj tego konfiguracji na produkcję**.

## Budowa bez Dockera

Rozwiązanie projektów pod `src` korzysta z nadrzędnych `Directory.Build.props` i `Directory.Packages.props` całego repozytorium. Lokalnie z katalogu repo: `dotnet build demonstracje/66_WebApiRabbitRedisSql/src/Api/Api.csproj` oraz analogicznie Worker.

## Warto omówić ze studentami

Co oznacza 202? Dlaczego ACK po COMMIT? Czym różni się cache miss od nieistniejącego rekordu? Jak działa at-least-once i idempotencja? Co się stanie gdy SQL zatwierdzi zmianę, ale Redis lub RabbitMQ są niedostępne?
