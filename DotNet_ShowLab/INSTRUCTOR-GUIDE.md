# ShowLab — przewodnik prowadzącego

Każdy pokaz: **hipoteza (1 min) → demonstracja (3–10 min) → interpretacja (2–3 min)**. Pracuj z katalogu głównego repo. W razie problemów z Dockerem wykorzystaj pokazy SDK-only.

## Proponowany blok
1. 01: Czy LINQ jest zawsze szybsze niż pętla? Wykonaj 2 przebiegi, pokaż rozgrzewkę i wpływ JIT.
2. 02: Czy zaalokowane 100 MB = aktualnie zajęte 100 MB? Uruchom `transient`, `retained`, `loh`; omów LOH od ~85 KB.
3. 04: Czy inkrementacja `++` jest atomowa? Porównaj `++` i `Interlocked.Increment`.
4. 08: Czy retry zawsze naprawia awarię? Pokaż transient failure i awarię permanentną.
5. 11: Dwie przeglądarki z aktualizacjami real-time — po przygotowaniu Dockera.
6. 09: monitoruj żywy proces w `dotnet-counters`. Jeśli proces kończy się szybko, demo 02 udostępnia pauzę przez `Console.ReadLine()`.

Alternatywa: 03 `Task.WhenAll`, 05 serwer HTTP i 10 kontener.

## Zasady interpretacji
- `GC.GetTotalAllocatedBytes` jest licznikiem kumulacyjnym, a nie zużyciem fizycznej pamięci.
- `GC.CollectionCount` jest licznikiem kolekcji, nie liczbą żywych obiektów.
- Benchmarki: `Stopwatch` do ilustracji, BenchmarkDotNet do rzetelnych pomiarów.
- Race condition nie gwarantuje błędnego wyniku w każdym uruchomieniu.
- Retry nie może bezrefleksyjnie ponawiać operacji o nieznanej idempotencji.
- JWT token zawiera dane zakodowane, a nie zaszyfrowane; hasła przechowuje się przez specjalizowany password hasher.

## Checklist
- `dotnet --info` wskazuje .NET SDK 10.
- Port 5088 wolny dla 05, Docker Desktop działa tylko jeśli potrzebny.
- Powtórz każde demo przed zajęciami; infrastrukturalne wymagają osobnej weryfikacji.
