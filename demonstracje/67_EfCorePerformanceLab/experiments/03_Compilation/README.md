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

## Kontrola warunków i interpretacja

- `Test-Lab.ps1` musi potwierdzić identyczne odpowiedzi i polecenia: N+1 = 101, Include = 1, Projection = 1, Split = 2.
- Dla porównania Query Store: `Run-All-Benchmark.ps1` wymaga `QUERY_CAPTURE_MODE = ALL` i izolowanych pomiarów.
- Porównuj mediany EF Core *oddzielnie* od średniego SQL CPU na żądanie. Nie odejmuj jednej metryki od drugiej.
- `ContainerReadyMs` nie jest porównywalne wprost z wcześniejszym Startup Lab, ponieważ obejmuje mechanikę Docker Compose i kontrolę gotowości API.
- Powtórz pełne serie kilkukrotnie i zmieniaj kolejność wariantów, aby zmniejszyć wpływ temperatury, cache, obciążenia hosta i rozgrzania środowiska.
- Zanotuj: wersję Windows, CPU, RAM, `docker version`, `docker info`, obraz SQL i `SELECT @@VERSION`. Dotychczasowe stanowisko: Intel Core i5-1035G1, 4C/8T, RAM 7,81 GiB, Windows 11 Pro build 26300. Nie zakładaj identycznej wersji CU SQL na podstawie poprzedniego laboratorium.
- **Brak wyników = brak wniosków.** Skrypty przygotowują eksperyment; wartości powstaną dopiero po uruchomieniu na stanowisku.

## Native AOT: osobna bramka zgodności

**Nie przedstawiam Native AOT jako piątej gotowej, porównywalnej próbki.** EF Core 10 wymaga ostrożności przy AOT: prekompilacji zapytań, odpowiedniego modelu i weryfikacji dostawcy SQL Server, a minimalne API również musiałoby przejść analizę pod kątem zgodności z AOT. Samo ustawienie `PublishAot=true` nie wystarcza.

Dalszy etap to osobny test kompilacji Native AOT, analiza diagnostyki MSBuild/ILLink, weryfikacja wszystkich czterech endpointów i sprawdzenie identycznych wyników SQL. Dopiero po przejściu tych warunków można dodać Native AOT do porównania wydajnościowego. Nie należy interpretować ewentualnego błędu kompilacji jako wyniku benchmarku.

Dokumentacja techniczna: [ReadyToRun](https://learn.microsoft.com/dotnet/core/deploying/ready-to-run), [EF Core Native AOT (experimental)](https://learn.microsoft.com/ef/core/performance/nativeaot-and-precompiled-queries).
