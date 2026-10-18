# 04 — Logi, metryki, ślady

1. Uruchom `01_Pipeline_DI`; wykonaj kilka GET `/orders/42` i jeden GET `/fail`.
2. Porównaj nagłówek `X-Correlation-ID` z wpisem logu.
3. Opcjonalnie: `dotnet tool install --global dotnet-counters`, `dotnet-counters ps`, `dotnet-counters monitor -p <PID> System.Runtime`.
4. Rozszerzenie: `demonstracje/59_OpenTelemetrySql` (wymaga Docker i infrastruktury), `demonstracje/42_SeqLogging`.

**Rozróżniaj:** log = zdarzenie, metryka = wartość/licznik, trace = przebieg operacji przez komponenty. Pokazowy correlation ID z middleware nie jest pełnym distributed tracingiem ani automatycznym `Activity.TraceId`.

**Pytania:** co zrobiłbyś, gdy klient zgłasza HTTP 500? jak odróżnić problem API od problemu SQL Server?
