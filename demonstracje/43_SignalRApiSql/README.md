# Demo 43 — SignalR + API + SQL Server + klient nginx (3 obrazy)

Hub `/hubs/chat` zapisuje wiadomość w SQL i rozsyła do klientów.
Trzeci obraz to **nginx** ze stroną HTML (CDN SignalR JS).

| Kontener | Obraz | Port | Rola |
|---|---|---|---|
| `sqlserver` | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 | historia czatu |
| `api` | `demo43-api:local` (budowany) | 5345 | Minimal API + Hub |
| `client` | `demo43-client:local` (nginx) | 5346 | strona HTML |

Testy łączą się z hubem przez `WebApplicationFactory` — **bez Dockera**.

## Start na sali

```bash
cd demonstracje/43_SignalRApiSql
docker compose up --build
```

Otwórz http://localhost:5346 (strona woła API na :5345 — CORS włączony w demo).

```bash
curl http://localhost:5345/api/v1/messages
```

Hasło `sa` / `Demo43_StrongPass!`. Port 1433 może kolidować z innymi demami.
Klient HTML ładuje SignalR z CDN (sala potrzebuje sieci).

## Co pokazać studentom

1. Trzy obrazy.
2. `Send` na hubie → wiersz w SQL + broadcast `Receive`.
3. Test integracyjny z `HubConnection` (bez przeglądarki).

## Testy bez Dockera

```bash
dotnet test demonstracje/43_SignalRApiSql/tests/Demo43_SignalRApiSql.Tests.csproj
```
