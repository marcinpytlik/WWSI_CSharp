# Eksperyment 03 — kompilacja .NET a wydajność EF Core

**Powiązanie:** [Demonstracja 67 — EF Core Performance Lab](../../README.md). W tym etapie nie zmieniam modelu danych, implementacji N+1 / Include / Projection / SplitQuery ani SQL Server. Zmieniam tylko sposób publikacji aplikacji i jedną opcję runtime.

## Pytania badawcze

1. Czy JIT, ReadyToRun i Composite ReadyToRun zmieniają **medianę czasu tej samej operacji EF Core**?
2. Czy wyłączenie Tiered Compilation zmienia wyniki aplikacji?
3. Czy Query Store nadal widzi **te same SQL commands i podobną pracę silnika**?
4. Czy zmiany czasu rozruchu kontenera są spójne ze zmianami czasu operacji EF Core? To **osobne metryki**.

## Kontrolowane warianty

| Mode | Publikacja | Tiered Compilation |
|---|---|---|
| `jit` | Release, self-contained linux-x64, bez R2R | włączona |
| `r2r` | Release, self-contained linux-x64, ReadyToRun | włączona |
| `composite` | Release, self-contained linux-x64, Composite ReadyToRun | włączona |
| `no-tiering` | identyczna publikacja jak `jit` | wyłączona |

Wszystkie warianty używają tego samego Dockerfile, .NET SDK 10, zestawu zależności, bazy `EfPerformanceLab`, portu API `5087`, kompilacji Release, RID `linux-x64` i **self-contained**, bo Composite ReadyToRun bez tego ostatniego ustawienia byłby ignorowany.

**Ważne:** zmierzony przez skrypt `ContainerReadyMs` to czas od wydania polecenia Docker Compose do pierwszej prawidłowej odpowiedzi `/health`. Obejmuje orkiestrację, uruchomienie procesu i inicjalizację aplikacji. **Nie jest to czysty czas startu CLR ani zimny start procesu.** Budowanie obrazu odbywa się osobno przed tym pomiarem.

## Uruchomienie z głównego katalogu Demo 67

Najpierw upewnij się, że działa oryginalne laboratorium i Query Store ma `READ_WRITE` / `ALL`. Nie trzeba usuwać istniejącego wolumenu SQL.

```powershell
cd WWSI_CSharp/demonstracje/67_EfCorePerformanceLab

docker compose up -d sql
docker compose ps

./experiments/03_Compilation/Run-Experiment.ps1 -Take 100 -Repetitions 10
```

Do pierwszego sprawdzenia kompilacji możesz użyć:

```powershell
./experiments/03_Compilation/Run-Experiment.ps1 -Modes jit,r2r -Repetitions 3 -SkipQueryStore
```

Po poprawnym przebiegu dla każdego trybu otrzymasz: potwierdzenie liczby SQL commands i zgodności danych, wynik benchmarku, CSV Query Store (jeśli nie użyto `-SkipQueryStore`) oraz zbiorczy CSV z metryką gotowości kontenera i medianami czterech endpointów. Skrypt zapisuje pliki tylko w `experiments/03_Compilation/results/`.

**Nie uruchamiaj podczas eksperymentu innych klientów Demo 67.** Pomiar następuje po kolejnym zastąpieniu tego samego serwisu `api`; serwis `sql` i dane pozostają bez zmian. Skrypt nie zatrzymuje ani nie resetuje SQL Server, ale po zakończeniu pozostawia aktywny ostatnio testowany wariant API. Aby przywrócić oryginalny obraz API:

```powershell
docker compose up -d --no-deps --build --force-recreate api
```

W razie niepowodzenia budowania lub smoke testu eksperyment jest przerywany. Nie traktuj częściowego zestawu wyników jako kompletnego porównania.

## Druga faza: powtarzalność i kolejność wariantów

Pierwsza pełna seria wykazała różnice czasów pomiędzy JIT, ReadyToRun, Composite ReadyToRun i JIT bez Tiered Compilation. **Jedna seria nie wystarcza do przypisania różnic metodzie kompilacji**; dodatkowo zawsze uruchamiała warianty w tej samej kolejności.

Dlatego druga faza stosuje **cztery rundy z przeplataną, zrównoważoną kolejnością** (plan Williamsa). Każda metoda występuje raz na każdej pozycji; dla każdej kombinacji `mode × strategy` zachowujemy osobne BEFORE/AFTER w Query Store.

| Runda | Kolejność kompilacji |
|---|---|
| 1 | jit → r2r → no-tiering → composite |
| 2 | r2r → composite → jit → no-tiering |
| 3 | composite → no-tiering → r2r → jit |
| 4 | no-tiering → jit → composite → r2r |

Uruchom w katalogu `67_EfCorePerformanceLab`:

```powershell
git pull --ff-only
./experiments/03_Compilation/Run-Repeatability.ps1 -Rounds 4 -Take 100 -Repetitions 10
```

Zachowaj spokój obciążenia na hoście; nie wykonuj równocześnie innego benchmarku ani zapytań do API. W tym ustawieniu powstaje łącznie **640 próbek aplikacyjnych**: 4 rundy × 4 warianty kompilacji × 4 strategie × 10 wywołań. Po każdej rundzie skrypt zapisuje punkt kontrolny, a na końcu:

- `results/<batch>-rounds.csv` — wszystkie zestawienia z poszczególnych rund;
- `results/<batch>-manifest.csv` — czas rozpoczęcia i kolejność w każdej rundzie;
- `results/<batch>-rollup.csv` — mediana z median serii, ich zakres, mediana średnich CPU SQL oraz zakres logical reads.

**Uwaga statystyczna:** „mediana z median” to opisowy wskaźnik powtarzalności, a nie jedna mediana ze wszystkich 40 pomiarów i nie test istotności. Surowe próbki poszczególnych serii pozostają w plikach `results/*-raw.csv`. Porównanie wariantów powinno również uwzględniać ich pozycję w rundach i rozrzut wyników.

Skrypt zachowuje bazę SQL, nie wykonuje `docker compose down -v`, nie czyści Query Store i po zakończeniu pozostawia ostatnio uruchomiony wariant API. Przebieg nie usuwa wcześniejszych plików CSV.

## Kontrola warunków i interpretacja

- `Test-Lab.ps1` musi potwierdzić identyczne odpowiedzi i polecenia: N+1 = 101, Include = 1, Projection = 1, Split = 2.
- Dla porównania Query Store: `Run-All-Benchmark.ps1` wymaga `QUERY_CAPTURE_MODE = ALL` i izolowanych pomiarów.
- Porównuj mediany EF Core *oddzielnie* od średniego SQL CPU na żądanie. Nie odejmuj jednej metryki od drugiej.
- `ContainerReadyMs` nie jest porównywalne wprost z wcześniejszym Startup Lab, ponieważ obejmuje mechanikę Docker Compose i kontrolę gotowości API.
- Powtórz pełne serie kilkukrotnie i zmieniaj kolejność wariantów, aby zmniejszyć wpływ temperatury, cache, obciążenia hosta i rozgrzania środowiska.
- Zanotuj: wersję Windows, CPU, RAM, `docker version`, `docker info`, obraz SQL i `SELECT @@VERSION`. Dotychczasowe stanowisko: Intel Core i5-1035G1, 4C/8T, RAM 7,81 GiB, Windows 11 Pro build 26300. Nie zakładaj identycznej wersji CU SQL na podstawie poprzedniego laboratorium.
- **Brak wyników = brak wniosków.** Skrypty przygotowują eksperyment; wartości powstaną dopiero po uruchomieniu na stanowisku.

## Trzecia faza: diagnostyka CPU procesu, GC i alokacji

Po czterech rundach porównawczych nadal nie należy przypisywać wszystkich różnic czasowych wyłącznie JIT/R2R. W tej fazie uruchamiamy **osobny test diagnostyczny** z opcjonalnym endpointem `/diag/runtime` (dostępnym wyłącznie przy `Lab__Diagnostics=true`). Główna implementacja czterech strategii EF Core nie została zmieniona.

Z katalogu `67_EfCorePerformanceLab`:

```powershell
git pull --ff-only
docker compose up -d sql

./experiments/03_Compilation/Run-RuntimeDiagnostics.ps1 `
    -Take 100 `
    -Repetitions 30 `
    -WarmupPerStrategy 5
```

Najpierw można przetestować jeden wariant:

```powershell
./experiments/03_Compilation/Run-RuntimeDiagnostics.ps1 -Modes jit -Take 100 -Repetitions 5
```

Skrypt zbuduje aplikację dla kolejnych wariantów JIT / R2R / Composite / No-tiering; sprawdzi zgodność wyników, rozgrzeje każdą strategię, a następnie zbierze po **30 odczytów czasu EF Core na wariant i strategię** (łącznie 480 zmierzonych żądań). Zapisze dwa pliki w `results/`:

- `<token>-runtime-samples.csv` — czasy poszczególnych żądań;
- `<token>-runtime-summary.csv` — mediany, delta czasu CPU procesu, delta łącznych alokacji, liczby kolekcji GC generacji 0/1/2 oraz stan sterty i working set.

**Zakres metryk:** `Process.TotalProcessorTime`, `GC.GetTotalAllocatedBytes(false)` i `GC.CollectionCount` to liczniki **całego procesu**, nie tylko aktywnego żądania EF Core. Mierzymy przyrost między dwoma sondami `/diag/runtime`, więc wynik obejmuje także serwer HTTP, serializację JSON, ewentualną pracę w tle i koszt samych sond. `ProcessCpuMsPerRequest` i `ProcessAllocatedKBPerRequest` są średnimi przyrostami na żądanie, **nie osobno zmierzonym CPU/allocations jednej operacji**. `HeapSizeBytes` i working set to snapshoty, a nie przyrost alokacji. Zero kolekcji GC nie oznacza braku alokacji.

W odróżnieniu od poprzedniego testu ta faza **nie pobiera nowych statystyk Query Store** — weryfikuje SQL commands w odpowiedziach i pozwala porównać metryki procesu. Historyczne pomiary Query Store zachowujemy bez modyfikacji. Dla niezależnych wyników należy powtórzyć przebieg z inną kolejnością `-Modes`, bez dodatkowego obciążenia hosta.

Po ostatnim wariancie pozostaje włączony lokalny endpoint diagnostyczny (API nasłuchuje wyłącznie na `127.0.0.1` hosta). Aby przywrócić domyślną aplikację, bez endpointu diagnostycznego:

```powershell
docker compose up -d --no-deps --build --force-recreate api
```

Nie uruchamiaj dwóch skryptów benchmarku jednocześnie. Nie kasuj wolumenu SQL.

## Czwarta faza: EventPipe — dotnet-counters i dotnet-trace

Ta faza jest **profilowaniem**, nie próbą odtworzenia liczb z poprzednich benchmarków. Zebrane wcześniej 640 pomiarów i pomiary CPU/alokacji pozostają nienaruszone. Uruchamiamy oddzielny kontener `profiler` z narzędziami .NET i nie instalujemy ich w obrazie API. Profilowanie wymaga **wspólnej przestrzeni PID oraz współdzielonego `/tmp`**, ponieważ .NET zapisuje tam gniazdo diagnostyczne. Taką konfigurację dodaje plik `docker-compose.diagnostics.yml`.

Najpierw szybki test jednego wariantu z samymi licznikami (przy istniejącym `docker compose up -d sql`):

```powershell
git pull --ff-only

./experiments/03_Compilation/Run-EventPipeDiagnostics.ps1 `
    -Modes jit `
    -Take 100 `
    -Repetitions 10 `
    -CaptureSeconds 300 `
    -SkipTrace
```

Następnie pełny pomiar (każdy wariant posiada **osobną** sesję liczników i śladu):

```powershell
./experiments/03_Compilation/Run-EventPipeDiagnostics.ps1 `
    -Modes jit,r2r,composite,no-tiering `
    -Take 100 `
    -Repetitions 20 `
    -CaptureSeconds 300
```

**Jak działa skrypt:** buduje aplikację i narzędzia, odtwarza wyłącznie usługę `api`, uruchamia `profiler` w przestrzeni PID API, sprawdza widoczność procesu poleceniem `dotnet-counters ps`, wykonuje smoke test i warm-up. Następnie zbiera `System.Runtime` do CSV i osobno ślad EventPipe z profilami `dotnet-common` (m.in. GC/JIT/kompilacja) oraz `dotnet-sampled-thread-time` (próbkowanie wątków). Każdej sesji towarzyszy ten sam zestaw żądań `nplusone/include/projection/split`. Pomiary są rozdzielone: nie uruchamiamy dwóch narzędzi diagnostycznych jednocześnie.

Pliki wynikowe:

- `results/<token>-<mode>-counters.csv` — seria czasowa liczników runtime;
- `results/<token>-<mode>-trace.nettrace` — ślad EventPipe do przejrzenia w PerfView, Visual Studio lub konwersji przez `dotnet-trace`;
- `results/<token>-<mode>-<counters|trace>-workload.csv` — czasy operacji mierzone w czasie profilowania;
- `results/<token>-<mode>-<counters|trace>-capture.log` — log działania narzędzia;
- `results/<token>-manifest.csv` — kontrola kompletności i nazw plików.

**Ważne ograniczenia:**
- Narzędzia dołączają po starcie procesu i **nie obejmują jego inicjalizacji**; ślad nie jest pomiarem cold start/JIT startowego.
- `dotnet-counters` otrzymuje `q`, a `dotnet-trace` otrzymuje Enter przez PTY/FIFO **po zakończeniu wszystkich żądań workloadu**. Oba narzędzia zamykają sesję w trybie interaktywnym i zapisują plik. `CaptureSeconds` jest parametrem doradczym — po jego przekroczeniu skrypt zgłasza ostrzeżenie, ale nie ucina śladu. Jeśli plik jest pusty/brakujący, sprawdź `*-capture.log` i `*.exit`.
- Profilowanie samo wpływa na wykorzystanie zasobów; pomiarów `*-workload.csv` z tej fazy **nie należy bezpośrednio porównywać** z wcześniejszymi pomiarami bez narzędzi.
- Liczniki są próbkowane co około 1 s; zbieranie śladu obejmuje wszystkie żądania workloadu (o ile narzędzie faktycznie rozpoczęło kolekcję przed pierwszym żądaniem). Po zakończeniu porównaj `WorkloadSeconds` w manifeście, log przechwytywania i liczbę próbek. Same pliki `.nettrace` **nie dowodzą**, że GC czy JIT były przyczyną różnic. Trzeba najpierw przeanalizować zdarzenia i ich czas.
- `profiler` działa jako root wyłącznie w lokalnym laboratorium, aby połączyć się z procesem. Nie przenoś tej konfiguracji do produkcji.
- Nie wykonujemy `docker compose down -v`; wolumen SQL pozostaje nienaruszony. Nowy wolumen `lab_diag_tmp` zawiera tylko gniazda diagnostyczne.
- Ślady mogą zawierać nazwy metod, stosy oraz pośrednio informacje o aplikacji. Nie publikuj surowych artefaktów z wrażliwego środowiska; tutaj laboratorium pracuje na syntetycznych danych.

Po zakończeniu i obejrzeniu wyników przywróć standardowe API i usuń usługę profilującą bez dotykania SQL:

```powershell
docker compose -f docker-compose.yml -f docker-compose.compilation.yml -f docker-compose.diagnostics.yml --profile diagnostics stop profiler
docker compose -f docker-compose.yml -f docker-compose.compilation.yml -f docker-compose.diagnostics.yml --profile diagnostics rm -f profiler
docker compose up -d --no-deps --build --force-recreate api
```

Oficjalne źródła: [Diagnostics in Linux containers](https://learn.microsoft.com/dotnet/core/diagnostics/diagnostics-in-containers), [dotnet-counters](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-counters), [dotnet-trace](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-trace).

## Native AOT: osobna bramka zgodności

**Nie przedstawiam Native AOT jako piątej gotowej, porównywalnej próbki.** EF Core 10 wymaga ostrożności przy AOT: prekompilacji zapytań, odpowiedniego modelu i weryfikacji dostawcy SQL Server, a minimalne API również musiałoby przejść analizę pod kątem zgodności z AOT. Samo ustawienie `PublishAot=true` nie wystarcza.

Dalszy etap to osobny test kompilacji Native AOT, analiza diagnostyki MSBuild/ILLink, weryfikacja wszystkich czterech endpointów i sprawdzenie identycznych wyników SQL. Dopiero po przejściu tych warunków można dodać Native AOT do porównania wydajnościowego. Nie należy interpretować ewentualnego błędu kompilacji jako wyniku benchmarku.

Dokumentacja techniczna: [ReadyToRun](https://learn.microsoft.com/dotnet/core/deploying/ready-to-run), [EF Core Native AOT (experimental)](https://learn.microsoft.com/ef/core/performance/nativeaot-and-precompiled-queries).


## Wyniki i zamknięcie serii 03D — 22.11.2026

Ukończono rejestrację czterech wariantów, 20 iteracji na strategię i rodzaj profilowania. Dla serii **3b971dd5** zapisano łącznie 640 pomiarów operacji EF Core i osiem plików diagnostycznych. Opublikowano raport [FINAL-REPORT-2026-11-22.md](FINAL-REPORT-2026-11-22.md) z wynikami i ograniczeniami metodologicznymi.

**Uwaga historyczna:** pierwotna seria 3b971dd5 korzystała ze sztywnego limitu 150 sekund dla `dotnet-trace`; podczas gdy workload trwał około 168–170 sekund. Dlatego wnioski raportu wynikają z pełnych CSV workloadu i liczników, **a nie z niekompletnych śladów .nettrace**. W aktualnej wersji skryptu usunięto to ograniczenie, ale ta poprawka wymaga osobnego testu integracyjnego po pobraniu nowego kodu.

Weryfikacja nowej obsługi trace, bez ponawiania całego czterowariantowego benchmarku:

```powershell
git pull --ff-only
./experiments/03_Compilation/Run-EventPipeDiagnostics.ps1 `
    -Modes jit `
    -Take 100 `
    -Repetitions 10 `
    -CaptureSeconds 300
```

Oczekiwane: `PASS jit counters`, `PASS jit trace`, pliki CSV i `.nettrace`; z manifeście powinny wynikać wartości `WorkloadSeconds` dla obydwu sesji. W tym eksperymencie nie są automatycznie publikowane surowe wyniki.
