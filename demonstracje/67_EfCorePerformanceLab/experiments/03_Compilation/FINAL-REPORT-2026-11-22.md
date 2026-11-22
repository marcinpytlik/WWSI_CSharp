# Demo 67 — raport końcowy: EF Core oczami DBA
**Data pomiarów:** 22 listopada 2026 · **Środowisko:** .NET 10, EF Core 10, SQL Server 2022, Docker Desktop, PowerShell 7

> **Teza:** w badanym przypadku strategia dostępu do danych EF Core wpływała na koszt operacji silniej niż różnice między rozgrzanymi wariantami publikacji/kompilacji .NET 10. To obserwacja lokalnego laboratorium, nie uniwersalny ranking sposobów kompilacji.

## 1. Cel i zakres

Porównano cztery implementacje odczytu identycznych danych — **100 zamówień, 1000 pozycji**, zawsze `AsNoTracking()`. Weryfikacja poprawności obejmowała liczbę poleceń SQL i zgodność DTO.

| Strategia | Liczba SQL na żądanie | Uwagi |
|---|---:|---|
| N+1 | 101 | 1 SELECT zamówień + 100 SELECT-ów pozycji |
| Include | 1 | `Include` + `AsSingleQuery()` |
| Projection | 1 | `Select` bezpośrednio do DTO |
| SplitQuery | 2 | `Include` + `AsSplitQuery()` |

Zebrano: pomiary aplikacyjne i Query Store, cztery zrównoważone rundy wariantów kompilacji (oddzielny etap 03B), liczniki procesu (03C), a następnie serię 03D: dwa rodzaje profilowania × cztery tryby publikacji × cztery strategie × 20 powtórzeń = **640 wyników czasowych**.

## 2. Bazowe porównanie: aplikacja kontra SQL Server

Kontrolowana seria Query Store: po 10 powtórzeń, `take=100`. Mediana EF to czas samej operacji w aplikacji; CPU, duration i logical reads pochodzą z silnika, uśrednione na żądanie.

| Strategia | Mediana EF, ms | SQL/żąd. | CPU SQL, ms/żąd. | Duration SQL, ms/żąd. | Logical reads/żąd. |
|---|---:|---:|---:|---:|---:|
| N+1 | 119,83 | 101 | 21,40 | 21,43 | 1103 |
| Include | 6,01 | 1 | 1,95 | 1,95 | 14 |
| Projection | 5,11 | 1 | 2,73 | 2,73 | 14 |
| SplitQuery | 5,23 | 2 | 1,70 | 1,70 | 17 |

W tej próbie N+1 trwał **około 23,5× dłużej** od Projection (119,83 / 5,11), mimo że każde wywołanie zwraca identyczny wynik. Spadek logical reads wynosił z 1103 do 14 dla Projection. Najmniejsza mediana aplikacyjna nie odpowiadała najmniejszemu CPU SQL (to ostatnie miał SplitQuery). **Nie odejmujemy** średniego SQL duration od mediany EF i nie nazywamy takiej różnicy „kosztem EF/JIT”.

## 3. Profilowanie procesu .NET — seria 03D

Źródło: lokalnie wyeksportowane `3b971dd5-analysis.zip` (13 plików CSV), 20 żądań dla każdej kombinacji `mode × strategy × capture`. Zapisane ślady `.nettrace` **nie były analizowane** w niniejszym raporcie.

### Mediana czasu EF podczas `dotnet-counters` (ms)

| Strategia | jit | r2r | composite | no-tiering |
|---|---:|---:|---:|---:|
| N+1 | 135,13 | 177,27 | 136,99 | 169,22 |
| Include | 6,27 | 7,39 | 6,64 | 9,79 |
| Projection | 5,76 | 6,75 | 6,09 | 6,20 |
| SplitQuery | 6,46 | 6,67 | 6,55 | 7,71 |

### Mediana czasu EF podczas `dotnet-trace` (ms)

| Strategia | jit | r2r | composite | no-tiering |
|---|---:|---:|---:|---:|
| N+1 | 137,43 | 145,46 | 142,01 | 178,11 |
| Include | 8,00 | 7,35 | 8,51 | 11,06 |
| Projection | 6,05 | 6,66 | 6,89 | 7,48 |
| SplitQuery | 6,67 | 6,17 | 6,52 | 7,66 |

Nie interpretuj tabel jako rankingów JIT kontra R2R: profilowanie obciąża proces, kolejność wariantów nie była kontrbalansowana, a pomiary odbywały się w ustalonym porządku strategii.

### Liczniki procesu (`dotnet-counters`)

| Tryb | Alokacje MB* | JIT time s* | JIT methods* | CPU user s* | Mediana working set MB | GC Gen0* |
|---|---:|---:|---:|---:|---:|---:|
| jit | 124,4 | 3,043 | 3752 | 4,84 | 185,4 | 7 |
| r2r | 123,0 | 2,924 | 3278 | 5,16 | 188,7 | 7 |
| composite | 124,0 | 2,855 | 2808 | 4,86 | 199,3 | 8 |
| no-tiering | 125,3 | 0,039 | 63 | 3,05 | 164,3 | 8 |

\* Wartości agregowane z interwałów około 1 s są **przybliżeniem** zależnym od definicji konkretnego licznika; nie są przypisane poszczególnym endpointom. Working set to wartość stanu procesu, nie suma alokacji.

Zwraca uwagę niewielka aktywność kompilacji metod w okresie rejestracji `no-tiering`, współwystępująca z często dłuższymi czasami operacji. To nie dowodzi, że wyłączenie Tiered Compilation jest przyczyną wzrostu czasu ani że JIT zawsze jest szybszy.

## 4. Ograniczenia metodologiczne

1. **Sztywny limit śladu w serii historycznej.** `dotnet-trace --duration 150s` kończył rejestrowanie, podczas gdy 20 powtórzeń workloadu trwało około 168–170 s. CSV z czasami żądań są kompletne, ale `.nettrace` nie pokrywa pełnej serii. Po tej obserwacji skrypt zmieniono na zakończenie **Enter po workloadzie**, przy użyciu PTY/FIFO. **Test integracyjny na stanowisku zakończył się PASS** 22.11.2026: token `b8e2a335`, counters CSV 266456 B i trace NETTRACE 26108814 B. Wciąż nie analizowano wewnętrznej osi czasu zdarzeń tego śladu.
2. **Ślady EventPipe nie zostały odczytane.** Nie ustalamy przyczyn większego pliku Composite (około 157 MB) bez analizy samych zdarzeń.
3. **Koszt profilowania.** Wyników z 03D nie porównujemy jeden do jednego z czasami 03A/03B bez profilerów.
4. **Kolejność i liczba powtórzeń.** 03D wykonano w kolejności jit → r2r → composite → no-tiering; strategia N+1 → Include → Projection → Split. Nie jest to samodzielna próba statystycznego wykazania przewagi kompilacji.
5. **Własność metryk.** Query Store pokazuje pracę silnika, `Stopwatch` zmierzył czas operacji wewnątrz API, a `System.Runtime` liczniki **całego procesu**. Nie można sumować ich w jedną miarę „kosztu EF Core”.
6. **Rozgrzewka.** EventPipe dołączono po uruchomieniu i warm-up; nie badano zimnego startu CLR ani kosztu początkowego JIT.

## 5. Wnioski

- Problem **N+1** nie polega na jednym wyjątkowo drogim SQL, lecz na liczbie poleceń, powtarzanych operacjach i round-tripach. W laboratorium okazał się dominującym czynnikiem czasu.
- `Include`, `Projection` i `SplitQuery` miały zbliżony rząd wielkości czasów; wybór między nimi zależy od kształtu danych, liczby kolekcji i wymagań kontraktu.
- SQL CPU i aplikacyjna latencja opisują **różne perspektywy** i nie muszą wskazywać tego samego najlepszego wariantu.
- Nie stwierdzono na podstawie 03D uniwersalnego zwycięzcy wśród JIT, R2R, Composite i no-tiering.
- Wymagana jest osobna weryfikacja pełnego trace i analiza zdarzeń, zanim postawimy hipotezy o GC/JIT jako przyczynach różnic.

## 6. Odtworzenie i następne kroki

1. Uruchom `scripts/Test-Lab.ps1` i `scripts/Run-All-Benchmark.ps1`, sprawdź `QUERY_CAPTURE_MODE = ALL`.
2. Dla powtarzalności kompilacji użyj `experiments/03_Compilation/Run-Repeatability.ps1`; dla diagnostyki `Run-EventPipeDiagnostics.ps1`.
3. Zakończono weryfikację automatycznego zamykania `dotnet-trace` na trybie `jit` (`b8e2a335`, PASS). Osobnym zadaniem jest analiza osi czasu zdarzeń zapisanego śladu.
4. Surowe artefakty nie są automatycznie publikowane. Nie umieszczaj w repozytorium `.env`, haseł ani dużych plików `.nettrace`.

**Źródła danych:** `README.md` Demo 67, raport pomiarów Query Store z 22.11.2026, lokalna seria `3b971dd5`, eksport `3b971dd5-analysis.zip`. Raport dokumentuje **dokonane pomiary** oraz test integracyjny poprawionego mechanizmu zapisu śladu; nie opisuje analizy zawartości `.nettrace`.
