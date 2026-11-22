# 67 — EF Core Performance Lab: N+1 vs Include vs Projection vs SplitQuery

**WWSI / .NET 10 / EF Core 10 / SQL Server 2022 / Docker Compose / PowerShell 7**

Cel: porównać cztery sposoby odczytu **tych samych danych** i zobaczyć, jak decyzja programisty zmienia obciążenie SQL Server. To jest przykład do nauki, nie benchmark porównawczy różnych silników baz danych.

## Scenariusz

Seed dodaje **100 zamówień po 10 pozycji = 1100 wierszy**. Każdy endpoint zwraca tę samą strukturę `OrderDto` z kolekcją `OrderItemDto`, w tej samej kolejności. Przy porównaniu `take=100` wszystkie cztery strategie powinny zwracać 100 zamówień i 1000 pozycji.

| Endpoint | Technika | Oczekiwana liczba SELECT przy take=100 |
|---|---|---:|
| /api/orders/nplusone | Jeden SELECT zamówień + osobny SELECT pozycji dla każdego zamówienia (antywzorzec) | 101 |
| /api/orders/include | `Include(x => x.Items)` + `AsSingleQuery()` | 1 |
| /api/orders/projection | Projekcja `Select` bez materializowania encji | 1 |
| /api/orders/split | `Include` + `AsSplitQuery()` (zapytanie główne i kolekcja) | 2 |

N+1 jest **celowo zaprogramowane jawną pętlą**, bez lazy-loading proxies, tak by efekt był deterministyczny. Dla porównania są używane `AsNoTracking()`, uporządkowanie i identyczne DTO. Zapytania mają znaczniki `TagWith("EFPLAB:...")`, przydatne np. w logach EF, lecz Query Store nie musi zachować tych komentarzy. Snapshoty rozpoznają kształt SQL bez przywiązywania się do `query_id`. Wyniki latency, logical reads i CPU **należy zmierzyć**, a nie traktować obrazka promocyjnego jako wyniku eksperymentu.

## Uruchomienie

Wymagania: Docker Desktop (Linux containers), Docker Compose, PowerShell 7; opcjonalnie SSMS. Projekt uruchamia się bez lokalnego .NET SDK, bo API jest kompilowane w kontenerze.

~~~powershell
cd demonstracje/67_EfCorePerformanceLab
Copy-Item env.template .env
# Zmień SQL_SA_PASSWORD w .env (silne hasło, tylko lokalne demo)
docker compose config --quiet
docker compose up -d --build
docker compose ps
Invoke-RestMethod http://localhost:5087/health
Invoke-RestMethod 'http://localhost:5087/api/orders/nplusone?take=100' | Select-Object strategy,orders,sqlCommands,elapsedMs
Invoke-RestMethod 'http://localhost:5087/api/orders/include?take=100' | Select-Object strategy,orders,sqlCommands,elapsedMs
Invoke-RestMethod 'http://localhost:5087/api/orders/projection?take=100' | Select-Object strategy,orders,sqlCommands,elapsedMs
./scripts/Test-Lab.ps1 -Take 100
./scripts/Run-Benchmark.ps1 -Repetitions 5 -Take 100
~~~

SQL Server jest wystawiony wyłącznie na localhost:14367, API na localhost:5087. Baza: `EfPerformanceLab`. Przy pierwszym uruchomieniu API tworzy tabele przez `EnsureCreatedAsync` i wykonuje idempotentny seed (tylko gdy brak zamówień). Po wstępnym uruchomieniu SQL Server i API mogą potrzebować chwili na inicjalizację. W przypadku problemów zobacz `docker compose logs api sql`.

**UWAGA:** Laboratorium używa konta `sa` wyłącznie dla prostoty instalacji i samo tworzy bazę. Nie jest to wzorzec wdrożenia produkcyjnego: tam wymagane są kontrolowane migracje, least privilege, TLS i sekrety. Nie uruchamiaj razem z cudzymi danymi SQL, nie wystawiaj portów poza localhost. Hasła w istniejącym wolumenie nie zmieniaj przez samo edytowanie .env. **Nie używaj `docker compose down -v`**, jeżeli chcesz zachować dane.

## Co mierzymy?

- `sqlCommands`: liczba poleceń wykonujących reader w EF Core dla danego żądania, dzięki `DbCommandInterceptor` i `AsyncLocal`. Nie jest to liczba wszystkich zapytań na serwerze.
- `elapsedMs`: czas materializacji wyniku i logiki endpointu mierzony `Stopwatch`. Nie obejmuje transmisji HTTP / serializacji odpowiedzi.
- `Data`: te same zamówienia i pozycje dla wszystkich strategii.
- `QueryStore.sql`: historyczne średnie CPU, czas trwania i logical reads *per zapytanie*. Nie sumuj ich bezmyślnie bez uwzględnienia liczby wykonań: N+1 to wiele krótkich poleceń. Query Store może agregować warianty i raportuje pomiary silnika SQL, nie HTTP.

## Zmierzone wyniki — 22.11.2026

Zobacz [RESULTS-2026-11-22.md](RESULTS-2026-11-22.md): przy 20 powtórzeniach mediana N+1 = **114,93 ms**, Include = **6,63 ms**, Projection = **6,20 ms**. W tamtej serii nie było jeszcze wariantu SplitQuery. Statystyki Query Store historyczne: średnie 11 logical reads dla zapytania pozycji, 14 dla Include i 14 dla Projection. **Nie są to porównania kosztów pojedynczego, zsynchronizowanego żądania**.

## Kontrolowany eksperyment z przyrostami Query Store

1. W SSMS połącz się z lokalnym `localhost,14367`, bazą `EfPerformanceLab`. Włącz na potrzeby labu `QUERY_CAPTURE_MODE = ALL`:

~~~sql
ALTER DATABASE EfPerformanceLab SET QUERY_STORE
( OPERATION_MODE = READ_WRITE, QUERY_CAPTURE_MODE = ALL );
~~~

2. Zbuduj najnowszą wersję API z tagami zapytań:

~~~powershell
docker compose up -d --build
./scripts/Test-Lab.ps1 -Take 100    # sprawdza 101 / 1 / 1 / 2 i zgodność danych
~~~

3. Rozgrzej warianty **przed** snapshotem (skrypt Test-Lab jest wystarczającym warm-up). W SSMS wykonaj [sql/01_QueryStoreSnapshot.sql](sql/01_QueryStoreSnapshot.sql) z `@Label = N'BEFORE'`. Skrypt powinien pokazać kategorie zapytań, a nie zero wierszy.
4. Bez innych klientów uruchom **tylko jedną strategię**, np.:

~~~powershell
./scripts/Run-Benchmark.ps1 -Take 100 -Repetitions 10 -SkipWarmup -Strategy nplusone
~~~

5. W SSMS wykonaj ponownie `sql/01_QueryStoreSnapshot.sql` po zmianie etykiety na `AFTER`. Następnie [sql/02_QueryStoreDelta.sql](sql/02_QueryStoreDelta.sql). Powtórz cały cykl BEFORE → osobna strategia → AFTER → delta dla `include`, `projection` i `split`.

**Dlaczego tak?** Query Store w zaobserwowanym środowisku nie zachował komentarzy `TagWith("EFPLAB:...")` w przechowywanym tekście SQL. Oryginalny filtr `LIKE '%EFPLAB:%'` dawał `BEFORE = 0`, `AFTER = 1` (przypadkowo przechwytując tekst samego zapytania diagnostycznego). Poprawione snapshoty rozpoznają rzeczywiste kształty zapytań SQL, używając `CHARINDEX` z dosłownymi fragmentami. Nie wymagają stałych `query_id`, ale **zależą od kształtu SQL generowanego przez tę wersję EF Core**.

`SHARED:ORDERS` oznacza SELECT wspólny dla N+1 i SplitQuery, dlatego należy uruchamiać pomiary osobno. Przy `take=100` i `Repetitions=10`: N+1 daje około 10 wykonań `SHARED:ORDERS` i 1000 `NPLUSONE:ITEMS`; SplitQuery około 10 `SHARED:ORDERS` i 10 `SPLIT:ITEMS`. Query Store gromadzi metryki asynchronicznie; przy opóźnieniach albo dodatkowym ruchu wynik może odbiegać od oczekiwań. Nie resetuj produkcyjnego Query Store.

## Zadania laboratoryjne

1. Uruchom 4 endpointy z `take=10`, a potem `take=100`. Jak rośnie liczba poleceń SQL?
2. Wykonaj warm-up i minimum 5 powtórzeń. Zapisz medianę, nie pojedynczy pomiar.
3. Porównaj liczby zamówień i pozycji między odpowiedziami.
4. Otwórz `sql/QueryStore.sql` w SSMS, wybierz bazę `EfPerformanceLab`, przeanalizuj powtarzane zapytania po `OrderId` i logical reads.
5. Zmień liczbę pozycji na zamówienie, powtórz eksperyment i oceń transfer danych w `Include`.
6. Porównaj `AsSplitQuery()` i oceń kompromis pomiędzy liczbą round-tripów a duplikowaniem kolumn w JOIN.

## Wnioski, które należy zweryfikować

- N+1 zwiększa liczbę round-tripów wraz z liczbą zamówień.
- `Include` pobiera pełne encje i ich kolumny; projekcja ogranicza listę kolumn.
- Jeden SELECT **nie gwarantuje** najlepszego planu i najkrótszego czasu. Dla wielu kolekcji JOIN może prowadzić do cartesian explosion.
- Nie porównuj wyłącznie czasu z jednego wykonania: cold/warm cache, plan cache, sieć, tracking i wielkość danych mają znaczenie.

## Bezpieczeństwo i sprzątanie

~~~powershell
docker compose down      # zachowuje wolumen SQL
# docker compose down -v  # DESTRUKCYJNE, usuwa bazę demo — tylko świadomy reset labu
~~~

> Jeśli używasz repozytorium jako materiału na zajęcia, najpierw sprawdź `docker compose up --build`, liczbę wierszy i payload oraz dopiero potem przedstawiaj studentom wnioski o wydajności.
