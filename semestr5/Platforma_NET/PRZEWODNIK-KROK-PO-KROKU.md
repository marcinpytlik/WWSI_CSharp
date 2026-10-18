# Semestr 5 — instrukcja krok po kroku do pięciu demonstracji

**WWSI, grupy 301/302 (jeśli realizują semestr 5) · .NET 10 · PowerShell 5.1/7 · 90 minut**

To podręcznik demonstracji, nie kolejny kurs składni C#. Wszystkie komendy zaczynamy **z katalogu głównego `WWSI_CSharp`**, chyba że zaznaczono inaczej. Uruchom aplikację w pierwszym terminalu, a żądania wysyłaj w drugim. `Ctrl+C` zatrzymuje serwer przed kolejnym pokazem.

## 0. Przygotowanie (przed zajęciami)

```powershell
git status
dotnet --info
dotnet --list-sdks
dotnet --list-runtimes
docker version           # opcjonalnie, jeśli planujesz kontenery
```

Sprawdź, czy wyświetla się SDK .NET 10. Porty 5000, 5050 i 8080 powinny być wolne; możesz zmienić je w poleceniach. **Nie wykonuj `git reset --hard`**, jeśli masz lokalne zmiany. Do części API / JWT wystarcza .NET SDK. Docker, Jaeger i SQL Server są opcjonalne.

---

## DEMO 01 — HTTP, middleware, routing i Dependency Injection (20 min)

**Cel:** zrozumieć drogę żądania i to, dlaczego `scoped` tworzy instancję na zakres HTTP requestu.

### Krok 1 — uruchom serwer (terminal A)

```powershell
dotnet run --project semestr5/Platforma_NET/01_Pipeline_DI/src/PipelineDemo.csproj --urls http://localhost:5000
```

**Oczekiwane:** w logu pojawi się `Now listening on: http://localhost:5000`. Terminal pozostaje zajęty: to poprawne.

### Krok 2 — wyślij trzy żądania (terminal B)

```powershell
$base = 'http://localhost:5000'
1..3 | ForEach-Object { Invoke-RestMethod "$base/orders/42" }
```

**Oczekiwane:** `id=42`, `status=Created`; za każdym razem inne `scopeId` i `correlationId`. To nie są identyfikatory studenta/zamówienia, tylko znaczniki cyklu życia i żądania.

### Krok 3 — pokaż nagłówki

```powershell
$r = Invoke-WebRequest "$base/orders/42" -UseBasicParsing
$r.StatusCode
$r.Headers['X-Correlation-ID']
$r.Content
```

**Oczekiwane:** HTTP 200, nagłówek `X-Correlation-ID` taki jak `correlationId` w JSON tej samej odpowiedzi.

### Krok 4 — porównaj z logami (terminal A)

Znajdź dla *jednego* `correlationId` wpisy `START`, `Endpoint`, `END`. Sprawdź, czy `scope` w logach jest taki sam jak `scopeId` w odpowiedzi.

**Wytłumacz:** `AddScoped<RequestStamp>()` tworzy jedną instancję w obrębie żądania; każde kolejne HTTP request otrzymuje nową. `correlationId` powstaje u nas ręcznie w middleware, więc nie jest automatycznie identyfikatorem standardowego distributed trace.

### Krok 5 — wywołaj błąd

```powershell
try {
    Invoke-RestMethod "$base/fail" -ErrorAction Stop
}
catch {
    $code = [int]$_.Exception.Response.StatusCode
    "HTTP: $code"
    if ($_.ErrorDetails.Message) { $_.ErrorDetails.Message }
}
```

**Oczekiwane:** HTTP 500, JSON z `error` i `correlationId`; stack trace wyłącznie w logu aplikacji. Jeśli PowerShell nie wyświetli body odpowiedzi, pokaż je `curl.exe -i http://localhost:5000/fail` (curl wyświetli też nagłówki).

### Krok 6 — zajrzyj do kodu

Plik `semestr5/Platforma_NET/01_Pipeline_DI/src/Program.cs`. Pokaż: `AddScoped`, `app.Use(...)`, `await next(context)`, `MapGet`, `catch` i `finally`.

**Pytania:** Co się stanie po usunięciu `await next`? Dlaczego kolejność middleware ma znaczenie? Czy HTTP 500 oznacza wyciek szczegółów wyjątku do klienta?

---

## DEMO 02 — EF Core, LINQ i SQL (15 min)

**Cel:** zrozumieć, że EF Core jest biblioteką mapującą obiekty na dane, a SQL jest wykonywany przez provider i bazę.

### Krok 1 — pokaż istniejący projekt

Otwórz katalog `demonstracje/08_BibliotekaEf` (EF Core + SQLite, bez zewnętrznego serwera). Zidentyfikuj plik `.csproj` i kod źródłowy:

```powershell
Get-ChildItem demonstracje/08_BibliotekaEf -Recurse -Filter *.csproj
Get-ChildItem demonstracje/08_BibliotekaEf -Recurse -Filter *.cs
```

### Krok 2 — uruchom zgodnie z rzeczywistą strukturą

```powershell
$project = (Get-ChildItem demonstracje/08_BibliotekaEf -Recurse -Filter *.csproj | Where-Object { $_.FullName -notmatch '[\\/]obj[\\/]' } | Select-Object -First 1).FullName
dotnet run --project $project
```

Jeśli przykład ma wiele projektów, wybierz **projekt wykonywalny**, a nie bibliotekę lub testy. Wariant awaryjny: pokaż kod bez uruchamiania — nie zmieniaj bazy na sali.

### Krok 3 — pokaż ścieżkę zapytania

Znajdź `DbContext`, model encji, provider SQLite i zapytanie LINQ. Omów różnicę między zbudowaniem `IQueryable` a wykonaniem przez np. `ToList()`, `ToListAsync()` czy `FirstOrDefault()`.

Jeśli zapytanie jest relacyjne i masz w kodzie `IQueryable`, możesz demonstrować `ToQueryString()` (po dołączeniu odpowiedniej przestrzeni nazw EF Core) — **nie zakładaj**, że istniejący projekt już wypisuje SQL.

### Krok 4 — porównaj tracking

Otwórz istniejącą demonstrację `demonstracje/58_AsNoTracking`. Wskaż `AsNoTracking()`; opisz kompromis: odczyt bez śledzenia jest często tańszy, ale modyfikacje obiektów nie zostają automatycznie zapisane poprzez mechanizm śledzenia.

### Krok 5 — rozszerzenie z SQL Server (opcjonalne)

Jeśli SQL Server jest gotowy, użyj `demonstracje/12_EfCoreSqlLeastPrivilege` do omówienia konta odczytującego dane i uprawnień migracyjnych. **Nie** uruchamiaj migracji na produkcji.

**Pytania:** Czy każde LINQ produkuje SQL? Kiedy zapytanie trafia do DB? Skąd bierze się N+1 i jak je diagnozować?

---

## DEMO 03 — JWT: 401, 403, role (15 min)

**Cel:** odróżnić uwierzytelnienie od autoryzacji.

### Krok 1 — uruchom demo 33

```powershell
dotnet run --project demonstracje/33_JwtRole403/src --urls http://localhost:5050
```

Demo zawiera wyłącznie testowe konta/sekret w `appsettings.json`; nie wykorzystuj go w produkcji.

### Krok 2 — brak tokena = 401

```powershell
curl.exe -i http://localhost:5050/api/v1/me
```

**Oczekiwane:** 401 Unauthorized. Można też sprawdzić `/api/v1/admin/stats`.

### Krok 3 — zaloguj zwykłego użytkownika

```powershell
$base = 'http://localhost:5050'
$user = Invoke-RestMethod "$base/api/v1/auth/login" -Method Post -ContentType 'application/json' -Body '{"email":"user@wwsi.edu.pl","password":"user123"}'
$user.role
$userHeaders = @{ Authorization = "Bearer $($user.accessToken)" }
Invoke-RestMethod "$base/api/v1/me" -Headers $userHeaders
```

**Oczekiwane:** rola `User`, HTTP 200 na `/me`.

### Krok 4 — zwykły użytkownik próbuje wejść do admina

```powershell
curl.exe -i -H "Authorization: Bearer $($user.accessToken)" http://localhost:5050/api/v1/admin/stats
```

**Oczekiwane:** 403 Forbidden — token poprawny, ale brak wymaganej roli.

### Krok 5 — logowanie administratora

```powershell
$admin = Invoke-RestMethod "$base/api/v1/auth/login" -Method Post -ContentType 'application/json' -Body '{"email":"admin@wwsi.edu.pl","password":"admin123"}'
Invoke-RestMethod "$base/api/v1/admin/stats" -Headers @{ Authorization = "Bearer $($admin.accessToken)" }
```

**Oczekiwane:** HTTP 200 i wynik zawierający `role=Admin`.

### Krok 6 — znajdź odpowiedzialny kod

Otwórz `demonstracje/33_JwtRole403/src/Program.cs` i pokaż `AddJwtBearer`, `UseAuthentication()`, `UseAuthorization()`, `RequireAuthorization()` i `RequireRole("Admin")`.

**Pytania:** Co oznacza 401, a co 403? Czy payload JWT jest zaszyfrowany? Kto weryfikuje podpis?

---

## DEMO 04 — diagnostyka, liczniki, trace (15 min)

**Cel:** zidentyfikować problem żądania bez debuggera.

### Krok 1 — ponownie uruchom API (terminal A)

```powershell
dotnet run --project semestr5/Platforma_NET/01_Pipeline_DI/src/PipelineDemo.csproj --urls http://localhost:5000
```

### Krok 2 — wygeneruj ruch (terminal B)

```powershell
1..20 | ForEach-Object { Invoke-RestMethod http://localhost:5000/orders/42 | Out-Null }
curl.exe -i http://localhost:5000/fail
```

### Krok 3 — zidentyfikuj PID

```powershell
dotnet-counters ps
```

Jeżeli narzędzie nie jest zainstalowane:

```powershell
dotnet tool install --global dotnet-counters
```

Uruchom `dotnet-counters ps` ponownie; wybierz PID procesu hostującego `PipelineDemo`. Nie wybieraj losowego procesu `dotnet`.

### Krok 4 — obserwuj metryki

```powershell
dotnet-counters monitor -p <PID> System.Runtime
```

W powyższej komendzie zamień `<PID>` na rzeczywisty numer, np. `12345` — nawiasów nie wpisuj. W innym terminalu ponów serię żądań.

**Interpretacja:** `dotnet.gc.collections` — kolekcje GC; `dotnet.gc.heap.total_allocated` — skumulowane alokacje (nie aktualna pamięć procesu); licznik wyjątków i ruchu zależy od dostępnych instrumentów i wersji runtime. Mały ruch może nie zmienić zauważalnie części liczników.

### Krok 5 — korelacja błędu

Porównaj `X-Correlation-ID` i wpis logu `Błąd żądania correlation=...`. Pokaż, że log ma szczegóły wyjątku, a klient dostaje bezpieczny komunikat.

### Krok 6 — opcjonalny distributed tracing

Jeśli Docker i porty 5359/16686/4317 są dostępne:

```powershell
Push-Location demonstracje/59_OpenTelemetrySql
docker compose up --build -d
Pop-Location
curl.exe -X POST http://localhost:5359/api/v1/events -H "Content-Type: application/json" -d '{"name":"Sala OTEL"}'
```

Na Windows w PowerShell przy kłopotach z cytowaniem JSON użyj:

```powershell
Invoke-RestMethod http://localhost:5359/api/v1/events -Method Post -ContentType 'application/json' -Body '{"name":"Sala OTEL"}'
```

Otwórz `http://localhost:16686` w przeglądarce, wybierz serwis `demo59-api` i sprawdź trace. Zatrzymanie:

```powershell
Push-Location demonstracje/59_OpenTelemetrySql
docker compose down
Pop-Location
```

**Pytania:** Czym różni się log od metryki i od trace? Jak prześledzić żądanie HTTP 500?

---

## DEMO 05 — publikacja, runtime, Docker (15 min)

**Cel:** zrozumieć różnicę między kodem, artefaktem i uruchomioną usługą.

### Krok 1 — publikuj aplikację

```powershell
$proj = 'semestr5/Platforma_NET/01_Pipeline_DI/src/PipelineDemo.csproj'
dotnet publish $proj -c Release -o ./.artifacts/pipeline-api --self-contained false
Get-ChildItem ./.artifacts/pipeline-api
```

**Oczekiwane:** `PipelineDemo.dll`, pliki `.json` i inne artefakty. Build kompiluje projekt, publish przygotowuje pliki do wdrożenia.

### Krok 2 — uruchom opublikowaną wersję

```powershell
dotnet ./.artifacts/pipeline-api/PipelineDemo.dll --urls http://localhost:5000
```

W drugim terminalu:

```powershell
Invoke-RestMethod http://localhost:5000/health
```

**Oczekiwane:** `status=healthy`. Zatrzymaj Ctrl+C.

### Krok 3 — zbuduj obraz

Wymaga Docker Desktop w trybie Linux containers:

```powershell
docker build -f semestr5/Platforma_NET/05_Publikacja_Docker/Dockerfile -t wwsi-pipeline .
docker images wwsi-pipeline
```

### Krok 4 — uruchom usługę w kontenerze

```powershell
docker run --rm --name wwsi-pipeline -p 8080:8080 wwsi-pipeline
```

W drugim terminalu:

```powershell
Invoke-RestMethod http://localhost:8080/health
Invoke-RestMethod http://localhost:8080/orders/42
docker ps
docker logs wwsi-pipeline
```

**Oczekiwane:** HTTP 200, ta sama logika co uruchomiona lokalnie. Zatrzymaj Ctrl+C w terminalu kontenera.

### Krok 5 — pokaż Dockerfile

`FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build` służy do kompilacji. `FROM mcr.microsoft.com/dotnet/aspnet:10.0` to mniejszy obraz wykonawczy dla ASP.NET Core. W finalnym obrazie nie ma potrzeby instalować pełnego SDK.

**Pytania:** Gdzie jest uruchamiany kod? Co jest w kontenerze, a co na hoście? Czym różni się `sdk` od `aspnet`?

---

## Podsumowanie dla prowadzącego (ostatnie 10 minut)

1. Narysuj drogę: **klient HTTP → middleware → routing/endpoint → DI → dane → odpowiedź**.
2. Poproś studenta o interpretację 200, 401, 403 i 500.
3. Pokaż, jak `correlationId` łączy odpowiedź z logiem.
4. Przypomnij, że kod źródłowy, artefakt `publish` i kontener to różne warstwy.
5. Uzupełnij [kartę obserwacji](STUDENT-WORKSHEET.md).

## Diagnostyka w razie problemów

- **Port zajęty:** `Get-NetTCPConnection -LocalPort 5000 -State Listen`; zatrzymaj poprzedni serwer albo zmień `--urls`.
- **Brak .NET SDK 10:** `dotnet --info`; sprawdź instalację SDK i `global.json`.
- **HTTP 404:** sprawdź dokładną trasę i metodę GET/POST.
- **HTTP 401/403:** sprawdź token `Authorization: Bearer ...` i rolę; to różne wyniki.
- **Docker bez odpowiedzi:** `docker ps -a`, `docker logs wwsi-pipeline`, tryb Linux containers, mapowanie `8080:8080`.
- **`dotnet-counters` nie widzi procesu:** upewnij się, że API nadal działa oraz że użytkownik ma dostęp do procesu.
- **Pokaz EF Core nie startuje:** skorzystaj z kodu i gotowej demonstracji `58_AsNoTracking` bez zmieniania produkcyjnych baz.
