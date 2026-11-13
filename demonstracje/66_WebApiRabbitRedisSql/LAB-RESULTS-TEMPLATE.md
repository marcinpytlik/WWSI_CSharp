# Demo 66 — karta pomiarów do samodzielnego wypełnienia

**Operator:** __________  **Data / strefa:** __________  
**Host / OS:** __________  **Commit (`git rev-parse --short HEAD`):** __________  
**PowerShell:** __________  **Docker Compose:** __________  **.NET:** __________

## Przygotowanie

- [ ] `docker compose config --quiet` OK
- [ ] SQL/RabbitMQ/Redis healthy, API/Worker działają
- [ ] `dotnet test`: ____ zaliczonych / ____ wszystkich
- [ ] `Smoke-Test.ps1`: obie linie PASS

## Przebieg

| Test | Czas | OperationId | Kryterium zaliczenia | PASS/FAIL | Dowód / uwagi |
|---:|---|---|---|---|---|
| 1 | | | 202 bez RabbitMQ, Completed po jego powrocie | | |
| 2 | | | Pending bez Workera, Completed po restarcie | | |
| 3 | | | Redis offline HTTP 200/MISS, potem MISS/HIT | | |
| 4 | | | live 200, ready 503, POST 503, SQL healthy | | |
| 5 | | | jeden retry i Completed | | |
| 6 | | | JSON bez retry, 3 retry drugiego, DLQ +2 | | |
| 7 | | | 3 dostawy; SQL Products/Processed/Completed = 1/1/1 | | |
| 8 | | | SIGKILL po COMMIT; redelivered=True; SQL 1/1/1 | | |
| 9 | | | operator approve, publish confirm, source ACK, SQL Completed 1/1 | | |

## Szczegóły pomiaru

**Test 4:** readiness ____ s, POST ____ s  
**RabbitMQ:** DLQ baseline ____, DLQ final ____, messages_ready ____, unacked ____  
**SQL:** Pending ____, Failed ____, Unpublished Outbox ____  
**Test 7/8:** Products ____, ProcessedCommands ____, Completed ____  
**Test 9:** OperationId ____, PUBLISH_CONFIRMED [ ], SOURCE_ACKED [ ]  
**Logi i zrzuty:** ______________________________________

## Zakończenie

- [ ] Wszystkie usługi przywrócone, chaos wyłączony
- [ ] SQL/RabbitMQ/Redis healthy, API/Worker up
- [ ] Końcowy `Smoke-Test.ps1` PASS
- [ ] Pozostałe komunikaty w DLQ sklasyfikowane

**Wnioski / odchylenia:** ______________________________________
