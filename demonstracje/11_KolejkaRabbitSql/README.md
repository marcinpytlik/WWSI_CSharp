# Demo 11 — RabbitMQ + SQL Server

Producer wrzuca zdarzenie `OrderPlaced` na kolejkę. Consumer zdejmuje je z RabbitMQ
i zapisuje zamówienie do **SQL Server** (EF Core).

Testy jednostkowe **nie** wymagają Dockera — sprawdzają `OrderProcessor` na pamięciowym magazynie.

## Wymagania na sali

- Docker Desktop / Docker Engine
- .NET 10 SDK

## Start infrastruktury

```bash
cd demonstracje/11_KolejkaRabbitSql
docker compose up -d
```

SQL Server wstaje zwykle 15–30 s; worker ponawia połączenie sam.

- RabbitMQ UI: http://localhost:15672 (user/hasło: `demo` / `demo`)
- SQL Server: `localhost,1433` (sa / `Demo11_StrongPass!`)

To hasła **wyłącznie do lokalnego docker-compose**, nie do produkcji.

## Uruchomienie

Terminal 1 — worker:

```bash
dotnet run --project demonstracje/11_KolejkaRabbitSql/src/Consumer
```

Terminal 2 — publikacja:

```bash
dotnet run --project demonstracje/11_KolejkaRabbitSql/src/Producer -- SKU-42 3
```

W logach consumera i w tabeli `Orders` (baza `Demo11Orders`) pojawi się wiersz.

## Testy bez Dockera

```bash
dotnet test demonstracje/11_KolejkaRabbitSql/tests/Demo11_KolejkaRabbitSql.Tests.csproj
```

## Co pokazać studentom

1. Kolejka oddziela wydawcę od zapisu do bazy (Producer nie zna SQL).
2. Consumer jest workerem (`BackgroundService`).
3. Błędna wiadomość: `Nack` bez requeue (w demo — na sali omówić DLQ).
4. Logika domenowa (`OrderProcessor`) jest testowalna bez Rabbit/SQL.
