# SQLManiak — materiał do publikacji (wersja robocza)
**Cykl:** EF Core oczami DBA · **Tytuł:** 101 zapytań SQL kontra jedno. Czy JIT może uratować N+1?

## Krótki wpis LinkedIn — PL

W naszym laboratorium EF Core 10, .NET 10 i SQL Server 2022 wróciliśmy do pytania, które regularnie pojawia się na zajęciach:

**Czy optymalizacja .NET potrafi zrekompensować nieefektywny sposób pobierania danych?**

Dla tych samych 100 zamówień i 1000 pozycji porównałem:
- N+1 — 101 poleceń SQL;
- Include — 1 polecenie;
- Projection — 1 polecenie;
- SplitQuery — 2 polecenia.

W kontrolowanym pomiarze Query Store mediana operacji N+1 wyniosła **119,83 ms**, a Projection **5,11 ms**. Logical reads SQL Server: **1103 kontra 14**.

Poszliśmy dalej: JIT, ReadyToRun, Composite ReadyToRun i wyłączone Tiered Compilation, a do tego `dotnet-counters`, `dotnet-trace` i 640 pomiarów podczas profilowania.

**Wniosek?** W tym workloadzie wybór strategii EF Core był ważniejszy niż niewielkie różnice pomiędzy rozgrzanymi wariantami kompilacji .NET.

I jeszcze jedna rzecz: najkrótszy czas aplikacji **nie musi** oznaczać najmniejszego CPU po stronie SQL Server. Dlatego warto patrzeć jednocześnie na aplikację i Query Store.

Pełne skrypty, metodologia i ograniczenia pomiaru są w Demo 67:
https://github.com/marcinpytlik/WWSI_CSharp/tree/main/demonstracje/67_EfCorePerformanceLab

#EFCore #SQLServer #DotNet #QueryStore #Performance #SQLManiak

## Long-form — konspekt artykułu

**Tytuł roboczy:** „101 poleceń SQL kontra jedno: EF Core 10, Query Store i granice optymalizacji JIT”

1. Pytanie studentów: czy wolno skupiać się wyłącznie na czasie metody w C#?
2. Stanowisko: SQL Server 2022, EF Core 10, .NET 10, 100 zamówień × 10 pozycji, `AsNoTracking()`.
3. Implementacje czterech strategii: N+1 bez lazy-loading proxies, Include, Projection, SplitQuery; asercje identycznych DTO.
4. Tabela wyników bazowych z Query Store; rozdzielenie median EF i średnich CPU/duration/readów.
5. Dlaczego 101 round-tripów ma znaczenie nawet przy tanich SELECT-ach.
6. Eksperyment kompilacji: JIT/R2R/Composite/no-tiering, zrównoważone rundy kontra pojedynczy fixed-order profiling.
7. Co wnosi profilowanie: osobne workloady z counters i trace; nie mieszać metryk procesu z jedną operacją EF.
8. Ograniczenie historycznych śladów 150 s przy ~168–170 s workloadu; poprawka obsługi Enter po workloadzie, oczekująca na test integracyjny.
9. Wnioski dla DBA i programisty EF Core; kiedy SplitQuery może ograniczyć kartezjańskie mnożenie wierszy, a kiedy dodatkowy round-trip szkodzi.
10. Link do odtwarzalnego repo i raportu końcowego.

### Zasady publikacji
- Nie nazywać wyników profilowanych „baseline”.
- Nie ogłaszać zwycięzcy kompilacji na podstawie jednej fixed-order serii.
- Nie sugerować, że rozmiar `.nettrace` dowodzi wyższego CPU lub GC.
- Aktualizować status testu nowej obsługi trace dopiero po potwierdzonym PASS.
- Dane syntetyczne i skrypty są publiczne; **surowe ślady i dane lokalnego środowiska pozostają lokalne**.

**Status:** tekst do autorskiej akceptacji; nie został opublikowany na LinkedIn ani blogu.
