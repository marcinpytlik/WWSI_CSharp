# 67 — EF Core Performance Lab: N+1 vs Include vs Projection

**WWSI / .NET 10 / EF Core 10 / SQL Server 2022 / Docker Compose / PowerShell 7**

Cel: porównać trzy sposoby odczytu **tych samych danych** i zobaczyć, jak decyzja programisty zmienia obciążenie SQL Server. To jest przykład do nauki, nie benchmark porównawczy różnych silników baz danych.

## Scenariusz

Seed dodaje **100 zamówień po 10 pozycji = 1100 wierszy**. Każdy endpoint zwraca tę samą strukturę `OrderDto` z kolekcją `OrderItemDto`, w tej samej kolejności.

| Endpoint | Technika | Oczekiwana liczba SELECT przy take=100 |
|---|---|---:|
| /api/orders/nplusone | Jeden SELECT zamówień + osobny SELECT pozycji dla każdego zamówienia (antywzorzec) | 101 |
| /api/orders/include | `Include(x => x.Items)` + `AsSingleQuery()` | 1 |
| /api/orders/projection | Projekcja `Select` bez materializowania encji | 1 |

N+1 jest **celowo zaprogramowane jawną pętlą**, bez lazy-loading proxies, tak by efekt był deterministyczny. Dla porównania są używane `AsNoTracking()`, uporządkowanie i identyczne DTO. Wyniki latency, logical reads i CPU **należy zmierzyć**, a nie traktować obrazka promocyjnego jako wyniku eksperymentu.

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
./scripts/Test-Lab.ps1 -Take 100\n./scripts/Run-Benchmark.ps1 -Repetitions 5 -Take 100
~~~

SQL Server jest wystawiony wyłącznie na localhost:14367, API na localhost:5087. Baza: `EfPerformanceLab`. Przy pierwszym uruchomieniu API tworzy tabele przez `EnsureCreatedAsync` i wykonuje idempotentny seed (tylko gdy brak zamówień). Po wstępnym uruchomieniu SQL Server i API mogą potrzebować chwili na inicjalizację. W przypadku problemów zobacz `docker compose logs api sql`.

**UWAGA:** Laboratorium używa konta `sa` wyłącznie dla prostoty instalacji i samo tworzy bazę. Nie jest to wzorzec wdrożenia produkcyjnego: tam wymagane są kontrolowane migracje, least privilege, TLS i sekrety. Nie uruchamiaj razem z cudzymi danymi SQL, nie wystawiaj portów poza localhost. Hasła w istniejącym wolumenie nie zmieniaj przez samo edytowanie .env. **Nie używaj `docker compose down -v`**, jeżeli chcesz zachować dane.

## Co mierzymy?

- `sqlCommands`: liczba poleceń wykonujących reader w EF Core dla danego żądania, dzięki `DbCommandInterceptor` i `AsyncLocal`. Nie jest to liczba wszystkich zapytań na serwerze.
- `elapsedMs`: czas materializacji wyniku i logiki endpointu mierzony `Stopwatch`. Nie obejmuje transmisji HTTP / serializacji odpowiedzi.
- `Data`: te same zamówienia i pozycje dla wszystkich strategii.
- `QueryStore.sql`: historyczne średnie CPU, czas trwania i logical reads *per zapytanie*. Nie sumuj ich bezmyślnie bez uwzględnienia liczby wykonań: N+1 to wiele krótkich poleceń. Query Store może agregować warianty i raportuje pomiary silnika SQL, nie HTTP.

## Zadania laboratoryjne

1. Uruchom 3 endpointy z `take=10`, a potem `take=100`. Jak rośnie liczba poleceń SQL?
2. Wykonaj warm-up i minimum 5 powtórzeń. Zapisz medianę, nie pojedynczy pomiar.
3. Porównaj liczby zamówień i pozycji między odpowiedziami.
4. Otwórz `sql/QueryStore.sql` w SSMS, wybierz bazę `EfPerformanceLab`, przeanalizuj powtarzane zapytania po `OrderId` i logical reads.
5. Zmień liczbę pozycji na zamówienie, powtórz eksperyment i oceń transfer danych w `Include`.
6. Dodatkowo porównaj `AsSplitQuery()` i oceń kompromis pomiędzy liczbą round-tripów a duplikowaniem kolumn w JOIN.

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
