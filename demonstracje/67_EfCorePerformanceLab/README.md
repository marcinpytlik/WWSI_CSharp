# 67 — EF Core Performance Lab: N+1 vs Include vs Projection vs SplitQuery

**WWSI | .NET 10 | EF Core 10 | SQL Server 2022 | Docker Compose | PowerShell 7**

> **Cel:** samodzielnie uruchomić cztery warianty pobierania tych samych danych, porównać czas aplikacji, CPU SQL Server, logical reads i liczbę poleceń SQL.  
> **Zalecana droga:** Docker → Query Store → Test-Lab → Run-All-Benchmark → analiza CSV. Nie trzeba ręcznie wykonywać snapshotów Query Store.

## 1. Co badamy?

Baza `EfPerformanceLab` zawiera po pierwszej inicjalizacji **100 zamówień po 10 pozycji**: 1000 pozycji, razem 1100 rekordów. Każdy wariant zwraca te same zamówienia i pozycje jako DTO.

| Strategia | Endpoint | Sposób pobierania | Polecenia SQL przy `take=100` |
|---|---|---|---:|
| N+1 | `/api/orders/nplusone` | 1 SELECT zamówień + 100 osobnych SELECT-ów pozycji | **101** |
| Include | `/api/orders/include` | `Include()` i `AsSingleQuery()` | **1** |
| Projection | `/api/orders/projection` | `Select()` do DTO i `AsSingleQuery()` | **1** |
| SplitQuery | `/api/orders/split` | `Include()` i `AsSplitQuery()` | **2** |

N+1 jest celowo napisane jako jawna pętla, bez lazy-loading proxies. Każdy wariant używa `AsNoTracking()`, porządkuje pozycje i zwraca identyczną strukturę. Mechanizm `DbCommandInterceptor` z `AsyncLocal` liczy polecenia reader wykonywane w danym żądaniu.

Po ćwiczeniu student powinien umieć wytłumaczyć problem N+1, wskazać różnicę między `Include`, projekcją i split queries oraz odróżnić czas aplikacji od CPU i logical reads SQL Server.

## 2. Wymagania

- Windows 11, Docker Desktop w trybie **Linux containers** i Docker Compose; odpowiednia ilość pamięci dla SQL Server (zalecane minimum 4 GB dostępnego RAM).
- **PowerShell 7** (`pwsh --version`), Git i połączenie potrzebne do pobrania obrazów.
- SSMS — **opcjonalnie**, bo automatyczny skrypt wywołuje `sqlcmd` wewnątrz kontenera.
- Lokalny .NET SDK **nie jest potrzebny**: API jest kompilowane w kontenerze .NET SDK 10.

Porty na hoście: API `127.0.0.1:5087`, SQL Server `127.0.0.1:14367`. Nie wykorzystuj istniejącego produkcyjnego SQL Server.

### Pliki i ich rola

| Plik | Co robi |
|---|---|
| `docker-compose.yml`, `env.template` | uruchamiają SQL i API, konfigurują hasło, porty i wolumen |
| `src/PerformanceLab.Api/Program.cs` | model danych, seed i cztery endpointy |
| `scripts/Test-Lab.ps1` | sprawdza liczbę zapytań i zgodność zwracanych danych |
| `scripts/Run-Benchmark.ps1` | zwykły pomiar jednej lub wszystkich strategii |
| **`scripts/Run-All-Benchmark.ps1`** | **kompletne porównanie czterech strategii z Query Store** |
| `sql/01_QueryStoreSnapshot.sql` | snapshot liczników z etykietą BEFORE/AFTER |
| `sql/02_QueryStoreDelta.sql` | ręczne porównanie jednej pary snapshotów |
| `sql/03_QueryStoreCompareAll.sql` | agregacja czterech serii dla automatu |
| `sql/QueryStore.sql` | podgląd historii zapytań z Query Store |
| `RESULTS-2026-11-22.md` | pierwsza seria referencyjna sprzed dodania SplitQuery |

## 3. Instalacja i uruchomienie — wykonuj po kolei

**Krok 1. Pobierz kod** w PowerShell 7:

~~~powershell
git clone https://github.com/marcinpytlik/WWSI_CSharp.git
cd WWSI_CSharp/demonstracje/67_EfCorePerformanceLab
~~~

Jeżeli repo już masz, przejdź do `demonstracje/67_EfCorePerformanceLab` i użyj `git pull`. Wszystkie pozostałe polecenia uruchamiaj z tego katalogu.

**Krok 2. Utwórz `.env` — tylko przy pierwszym uruchomieniu.**

~~~powershell
Copy-Item env.template .env
notepad .env
~~~

Zmień wartość `SQL_SA_PASSWORD` na **własne silne hasło**; nie umieszczaj hasła w repozytorium ani w pracy zaliczeniowej. `.env` jest ignorowany przez Git. Jeśli baza była już uruchamiana, **nie nadpisuj istniejącego `.env`**: zmiana zmiennej nie zmienia hasła `sa` zapisanego w istniejącym wolumenie SQL Server.

**Krok 3. Zbuduj i uruchom kontenery.**

~~~powershell
docker compose config --quiet
docker compose up -d --build
docker compose ps
Invoke-RestMethod http://localhost:5087/health
~~~

Oczekiwane: `sql` ma status `healthy`, `api` jest `Up`, endpoint `/health` zwraca `ready`. Inicjalizacja API tworzy schemat poprzez `EnsureCreatedAsync` i wstawia dane, o ile nie ma jeszcze zamówień.

**Krok 4. Ustaw Query Store w trybie ALL — wymagany dla pełnego testu.**

W PowerShell 7, bez konieczności uruchamiania SSMS:

~~~powershell
docker compose exec sql bash -c 'export SQLCMDPASSWORD="$MSSQL_SA_PASSWORD"; /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -C -d master -b -Q "ALTER DATABASE EfPerformanceLab SET QUERY_STORE (OPERATION_MODE = READ_WRITE, QUERY_CAPTURE_MODE = ALL);"'
~~~

Lub w SSMS połącz się z `localhost,14367`, uwierzytelnianiem SQL Server, użytkownikiem `sa` i hasłem z `.env`, a następnie:

~~~sql
ALTER DATABASE EfPerformanceLab
SET QUERY_STORE (OPERATION_MODE = READ_WRITE, QUERY_CAPTURE_MODE = ALL);
GO
USE EfPerformanceLab;
GO
SELECT actual_state_desc, query_capture_mode_desc
FROM sys.database_query_store_options;
GO
~~~

Oczekiwane: `READ_WRITE` oraz `ALL`. Tryb `AUTO` może pominąć część krótkich zapytań i uniemożliwić porównanie wszystkich poleceń.

**Krok 5. Wykonaj smoke test.**

~~~powershell
./scripts/Test-Lab.ps1 -Take 100
~~~

Oczekiwane są wszystkie komunikaty `PASS`, w tym:

~~~text
PASS nplusone : 100 orders, 101 SQL commands
PASS include : 100 orders, 1 SQL commands
PASS projection : 100 orders, 1 SQL commands
PASS split : 100 orders, 2 SQL commands
PASS include : payload matches nplusone
PASS projection : payload matches nplusone
PASS split : payload matches nplusone
~~~

Test porównuje **dane** wszystkich czterech wariantów, a nie wyłącznie liczby zapytań. Jeśli nie przejdzie, najpierw diagnozuj błąd; nie wykonuj jeszcze benchmarku.

## 4. Jeden skrypt do kompletnego porównania

~~~powershell
./scripts/Run-All-Benchmark.ps1 -Take 100 -Repetitions 10
~~~

Działanie skryptu:

1. Test zgodności wyników i wstępna rozgrzewka (`Test-Lab.ps1`).
2. Osobna seria dla każdej strategii: snapshot **BEFORE**, 10 wywołań wybranej strategii, snapshot **AFTER**.
3. Pobranie przyrostów z Query Store i sprawdzenie kompletności poleceń SQL: N+1 = **1010**, Include = **10**, Projection = **10**, SplitQuery = **20**.
4. Zestawienie czasu aplikacji i kosztów SQL na **jedno żądanie**.
5. Eksport do CSV. Każde wywołanie automatu otrzymuje osobny identyfikator `token`.

W trakcie pomiaru nie uruchamiaj równolegle innych klientów tego laboratorium. Skrypt **nie resetuje historii Query Store** i nie usuwa bazy.

Tabela końcowa zawiera:

| Kolumna | Znaczenie |
|---|---|
| `Strategy` | nplusone / include / projection / split |
| `MedianEfMs` | mediana czasu operacji mierzonej `Stopwatch` w aplikacji |
| `SqlCommandsPerRequest` | liczba poleceń SQL na żądanie |
| `SqlCpuMsPerRequest` | **średnia** suma CPU SQL Server na żądanie |
| `SqlDurationMsPerRequest` | **średnia** suma duration instrukcji SQL na żądanie |
| `SqlLogicalReadsPerRequest` | **średnie** logical reads na żądanie |
| `Requests` | liczba zmierzonych wywołań danej strategii |

Pliki w `scripts`:

- `comparison-<token>.csv` — końcowe porównanie wszystkich strategii;
- `benchmark-all-<token>.csv` — każdy pojedynczy pomiar aplikacyjny;
- `benchmark-results.csv` — wynik ostatniej strategii, **nadpisywany** w trakcie testu.

Dwa pliki z `token` są właściwymi artefaktami do analizy i oddania prowadzącemu. Nie są automatycznie dodawane do Git.

**Ważne:** `MedianEfMs` to mediana, a metryki Query Store są średnimi na żądanie. Nie należy ich bezpośrednio odejmować od siebie i nazywać różnicy np. kosztem JIT, sieci lub materializacji. Stopwatch nie obejmuje serializacji odpowiedzi ani pełnego czasu HTTP. Logical reads to odczyty stron logicznych, a nie fizyczne odczyty dysku.

## 5. Wyniki referencyjne — 22 listopada 2026

W dotychczas sprawdzonej, kontrolowanej serii `Run-All-Benchmark.ps1` (po 10 powtórzeń, `take=100`) uzyskano:

| Strategia | Mediana EF (ms) | Polecenia / żądanie | CPU SQL (ms/żąd.) | Duration SQL (ms/żąd.) | Logical reads / żądanie |
|---|---:|---:|---:|---:|---:|
| N+1 | 119,83 | 101 | 21,40 | 21,43 | 1103 |
| Include | 6,01 | 1 | 1,95 | 1,95 | 14 |
| Projection | 5,11 | 1 | 2,73 | 2,73 | 14 |
| SplitQuery | 5,23 | 2 | 1,70 | 1,70 | 17 |

**Nie oczekuj identycznych milisekund** na swoim komputerze. Wyniki zależą od hosta, obciążenia, rozgrzania środowiska, pamięci, cache, planów wykonania i wersji obrazów. Przykład pokazuje, że najkrótszy czas aplikacji nie zawsze oznacza najmniejsze CPU SQL.

Starsza seria (20 powtórzeń, jeszcze bez SplitQuery) jest udokumentowana w [RESULTS-2026-11-22.md](RESULTS-2026-11-22.md).

## 6. Dla dociekliwych — ręczne testy i Query Store

**Szybki benchmark bez Query Store:**

~~~powershell
./scripts/Run-Benchmark.ps1 -Take 100 -Repetitions 10 -Strategy all
./scripts/Run-Benchmark.ps1 -Take 100 -Repetitions 10 -Strategy include
~~~

**Ręczny snapshot jednej strategii:**

1. Wykonaj `Test-Lab.ps1` i potwierdź `READ_WRITE` + `ALL` w Query Store.
2. Otwórz [sql/01_QueryStoreSnapshot.sql](sql/01_QueryStoreSnapshot.sql) w SSMS i uruchom z `@Label = N'BEFORE'`. Powinno być **pięć** kategorii: `NPLUSONE:ITEMS`, `SHARED:ORDERS`, `INCLUDE`, `PROJECTION`, `SPLIT:ITEMS`.
3. Uruchom tylko N+1:

~~~powershell
./scripts/Run-Benchmark.ps1 -Strategy nplusone -Take 100 -Repetitions 10 -SkipWarmup
~~~

4. W `01_QueryStoreSnapshot.sql` zmień etykietę na `AFTER` i uruchom skrypt ponownie.
5. Wykonaj [sql/02_QueryStoreDelta.sql](sql/02_QueryStoreDelta.sql). Oczekiwane przyrosty: około **1000 `NPLUSONE:ITEMS`** i **10 `SHARED:ORDERS`**.

**Dlaczego nie filtrujemy po `TagWith("EFPLAB:...")`?** W naszym SQL Server tekst przechowywany w Query Store nie zawierał tych komentarzy. Wcześniejszy filtr `LIKE '%EFPLAB:%'` zwracał 0 lub 1 wiersz (w tym sam skrypt diagnostyczny). Obecne snapshoty klasyfikują zapytania po **kształcie generowanego SQL**, nie po stałych `query_id`. To dopasowanie jest specyficzne dla tej wersji kodu i może wymagać aktualizacji po zmianie EF Core.

`SHARED:ORDERS` jest wspólnym kształtem odczytu zamówień dla N+1 i SplitQuery. Dlatego automatyczny benchmark izoluje strategie.

## 7. Typowe błędy i rozwiązania

| Problem | Co sprawdzić |
|---|---|
| `WARN The "MSSQL_SA_PASSWORD" variable is not set` | Poprzedni healthcheck interpolował hasło przed uruchomieniem kontenera. Zrób `git pull`, `docker compose config --quiet`, `docker compose up -d --force-recreate`. |
| `18456 State 8`, login `sa` | Niezgodność hasła w `.env` z kontem `sa`, szczególnie po ponownym użyciu wolumenu. Nie kasuj wolumenu automatycznie. |
| SQL `health: starting`, API `Created` | API czeka na SQL. Sprawdź `docker compose ps -a` oraz `docker compose logs sql api`. |
| Snapshot ma 0/1 wiersz | Sprawdź `QUERY_CAPTURE_MODE = ALL`, uruchom `Test-Lab.ps1` i użyj aktualnych plików SQL z Git. |
| `Invalid column name 'QueryId' / 'PlanId'` | Stara wersja `01_QueryStoreSnapshot.sql`; wykonaj `git pull`. |
| PowerShell `Nie można odnaleźć właściwości Count` | Stara wersja skryptu — poprawiono obsługę pojedynczych wyników; wykonaj `git pull`. |
| `sqlcmd` nie istnieje | Sprawdź `docker compose exec sql ls /opt/mssql-tools18/bin/sqlcmd`; wersja obrazu mogła się zmienić. |
| Port `5087` lub `14367` zajęty | Sprawdź `docker ps`, dostosuj mapowanie portu hosta i opcjonalny parametr `-BaseUrl` w skryptach. |
| Wyniki czasowe odbiegają od przykładu | Porównaj wersje obrazów i zasoby hosta; powtórz pomiary, analizuj mediany. |

## 8. Zadania studenta i kryteria zaliczenia

1. Uruchom test poprawności. Dlaczego liczba poleceń SQL to **101/1/1/2**?
2. Uruchom pełny benchmark dla `-Take 100 -Repetitions 10`, zapisz oba CSV z tokenem.
3. Policz, ile razy szybszy był wybrany wariant od N+1 (na podstawie median) i o ile procent zmniejszyły się logical reads.
4. Porównaj najmniejszą medianę czasu EF Core z najmniejszym średnim CPU SQL Server. Czy wskazują ten sam wariant?
5. Powtórz ćwiczenie z `-Take 10` i `-Take 50`; opisz skalowanie liczby poleceń.
6. Opisz, kiedy `AsSplitQuery()` może ograniczyć duplikację danych (szczególnie przy kilku kolekcjach), a kiedy dodatkowe round-tripy będą niekorzystne.
7. Opcjonalnie w SSMS przejrzyj `sql/QueryStore.sql`, identyfikuj instrukcje SQL i plany wykonania.

**Do oddania:** `comparison-<token>.csv`, `benchmark-all-<token>.csv`, krótki opis środowiska i wnioski do punktów 3–6. **Nie przesyłaj `.env` ani haseł.**

## 9. Eksperyment 03 — JIT / ReadyToRun / Composite / bez Tiered Compilation

Kolejnym, **oddzielnym etapem** jest badanie wpływu sposobu publikacji i kompilacji .NET na dokładnie te same cztery operacje EF Core. Przygotowałem cztery kontrolowane warianty, osobny Dockerfile i skrypt PowerShell zachowujący działający kontener SQL:

[**Eksperyment 03 — instrukcja, metodologia i skrypt**](experiments/03_Compilation/README.md)

~~~powershell
./experiments/03_Compilation/Run-Experiment.ps1 -Take 100 -Repetitions 10
~~~

Warianty: `jit`, `r2r`, `composite`, `no-tiering`. Wszystkie są publikowane jako **self-contained linux-x64**, aby Composite ReadyToRun naprawdę działał. Native AOT jest na razie **osobnym zadaniem sprawdzenia zgodności**, a nie piątą zmierzoną konfiguracją. Skrypt tworzy CSV w `experiments/03_Compilation/results/`; nie ma jeszcze wyników tej serii.

## 10. Zatrzymanie i bezpieczeństwo

~~~powershell
docker compose down       # zatrzymuje kontenery, zachowuje wolumen danych SQL
# docker compose down -v  # DESTRUKCYJNE — usuwa wolumen i bazę demonstracyjną
~~~

Laboratorium stosuje konto `sa` i automatyczne tworzenie schematu **wyłącznie dla prostoty zajęć**. Porty są wystawione tylko na `127.0.0.1`. Nie używaj tej konfiguracji w produkcji. To badanie strategii pobierania danych, **nie** porównanie JIT, ReadyToRun ani Native AOT — eksperyment kompilacji jest osobnym etapem.
