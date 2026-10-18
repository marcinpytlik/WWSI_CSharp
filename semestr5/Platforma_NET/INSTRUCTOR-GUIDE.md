# Prowadzący — pokaz 90 minut (semestr 5)

**Idea:** nie tworzymy kolejnego CRUD. Zadaj pytanie: co dzieje się od nadejścia HTTP do odpowiedzi i jak znaleźć błąd po wdrożeniu?

1. **0–20: pipeline.** Uruchom `01_Pipeline_DI`; wywołaj `/health`, `/orders/42`, `/fail`. Zwróć uwagę na nagłówek `X-Correlation-ID`, czasy w logach, różnicę 200/500, scoped `RequestStamp`. Użyj dwóch oddzielnych żądań. Pokaż dlaczego middleware musi wykonać `await next()`.
2. **20–35: EF.** Pokaż kod w `demonstracje/08_BibliotekaEf` lub `demonstracje/12_EfCoreSqlLeastPrivilege`; zaakcentuj `IQueryable`, wykonanie zapytania, `AsNoTracking`, logi SQL i problem N+1. Alternatywa: `demonstracje/58_AsNoTracking`.
3. **35–50: uwierzytelnianie.** Wykorzystaj `demonstracje/33_JwtRole403` (role i 401/403); nie wyświetlaj prawdziwych sekretów, tokeny wyłącznie demonstracyjne.
4. **50–65: diagnostyka.** Uruchom aplikację API, `dotnet-counters ps`, `dotnet-counters monitor -p <PID> System.Runtime` (opcjonalnie); porównaj logi skorelowane przez `X-Correlation-ID`. Dla tracingu rozważ istniejącą `demonstracje/59_OpenTelemetrySql` tylko z przygotowaną infrastrukturą.
5. **65–80: publikacja.** `dotnet publish`, potem zbuduj obraz z `05_Publikacja_Docker/Dockerfile`. Pokaż różnicę SDK i ASP.NET runtime.
6. **80–90: dyskusja.** Dlaczego 500? Która warstwa odpowiada za 401/403? Kiedy EF Core wysyła SQL? Jak skorelować żądanie z logiem?

**Przygotowanie:** przetestuj lokalne polecenia i port przed zajęciami; Docker, SQL Server, Jaeger są opcjonalne. Przy niepowodzeniu integracji z infrastrukturą pokaż kod i logi wcześniej przygotowanego przebiegu. Projekt pokazowy nie zawiera połączenia z produkcyjnymi systemami.
