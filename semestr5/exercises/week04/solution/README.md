# Semestr 5 — Tydzień 4: rozwiązania wzorcowe

Projekt **.NET 10 Minimal API + EF Core 10 + SQLite** realizuje [ćwiczenia tygodnia 4](../README.md), łącznie **100 pkt**. SQLite umożliwia pracę bez serwera bazodanowego. Projekt jest przykładem edukacyjnym; kod dostępu do danych można przenieść na SQL Server po zmianie providera, connection string i wygenerowaniu migracji specyficznych dla tego providera.

## Uruchomienie – Development

Wymagane .NET SDK 10. Uruchom polecenia w katalogu rozwiązania:

```powershell
cd semestr5/exercises/week04/solution
dotnet restore Week04.Solutions.slnx
dotnet build Week04.Solutions.slnx
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project Week04.Api --urls http://localhost:5104
```

W Development aplikacja wykonuje `MigrateAsync()`, następnie idempotentny seed trzech produktów. Plik SQLite `week04.development.db` zostaje utworzony względem katalogu roboczego procesu. Restart nie powoduje dodania kolejnych produktów.

W drugim oknie PowerShell 7:

```powershell
$base = "http://localhost:5104/api/v1/products"
Invoke-RestMethod $base | Format-Table
Invoke-RestMethod "$base/1" | Format-List
(Invoke-WebRequest -SkipHttpErrorCheck "$base/999").StatusCode # 404
```

## Zadania i kryteria

| Zadanie | Punkty | Implementacja |
|---|---:|---|
| 1. Pierwszy DbContext | 10 | `ProductsDbContext`, `DbSet<Product>`, DI, `UseSqlite`, connection string z konfiguracji |
| 2. Konfiguracja encji | 15 | `IEntityTypeConfiguration<Product>`: PK, required, długości, precision, unikalne SKU |
| 3. Pierwsza migracja | 20 | `InitialProducts` + model snapshot; instrukcja CLI |
| 4. Seed danych | 25 | `DevelopmentSeeder` tylko w Development, stałe SKU, brak duplikatów |
| 5. Wiele środowisk | 30 | appsettings dla Development/Production, User Secrets, env vars; brak haseł w Git |

## Migracje EF Core – CLI

Zainstaluj narzędzie w wersji zgodnej z EF Core 10 (jeśli nie jest dostępne):

```powershell
dotnet tool install --global dotnet-ef --version 10.0.0
# Jeśli już zainstalowano, sprawdź: dotnet ef --version
```

Z katalogu `solution`, przy `ASPNETCORE_ENVIRONMENT=Development`:

```powershell
# Lista migracji:
dotnet ef migrations list --project Week04.Api --startup-project Week04.Api

# Utworzenie kolejnej migracji po zmianie modelu:
dotnet ef migrations add AddProductDescription --project Week04.Api --startup-project Week04.Api

# Zastosowanie migracji:
dotnet ef database update --project Week04.Api --startup-project Week04.Api

# Usunięcie ostatniej NIEZASTOSOWANEJ migracji:
dotnet ef migrations remove --project Week04.Api --startup-project Week04.Api

# Wygenerowanie skryptu do przeglądu przed wdrożeniem:
dotnet ef migrations script --idempotent --project Week04.Api --startup-project Week04.Api --output migration.sql
```

**Uwaga:** `migrations remove` nie służy do cofania produkcyjnej bazy. Najpierw zaplanuj właściwy rollback/nową migrację oraz kopię zapasową. W repo jest już pierwsza migracja: nie wykonuj ponownie `add InitialProducts`.

## Konfiguracja Development i Production

- `appsettings.json`: ustawienia wspólne, bez sekretów.
- `appsettings.Development.json`: lokalna demonstracyjna SQLite, bez hasła.
- `appsettings.Production.json`: logowanie, bez connection string.
- `ConnectionStrings__Products`: zmienna środowiskowa nadpisująca konfigurację.
- User Secrets: lokalne dane wrażliwe poza repozytorium.

Przykład lokalnych sekretów (po wykonaniu `cd Week04.Api`):

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Products" "Data Source=week04.private.db"
```

Przykład **Production** (demo SQLite; w produkcji użyj trwałego wolumenu, kopii zapasowych, właściwych uprawnień i zarządzania sekretami):

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ConnectionStrings__Products = "Data Source=week04.production.db"
dotnet run --project Week04.Api --urls http://localhost:5104
```

**Production celowo nie uruchamia automatycznie migracji ani seeda.** Przed startem aplikacji przygotuj bazę kontrolowanym poleceniem `dotnet ef database update` (z konfiguracją produkcyjną) lub zaakceptowanym skryptem migracyjnym. Jeśli baza nie ma schematu, żądanie do `/api/v1/products` zakończy się błędem, co jest oczekiwanym sygnałem nieukończonego wdrożenia.

## Testy weryfikacyjne

1. Uruchom Development dwukrotnie. `GET /api/v1/products` powinien zwrócić **3 produkty** w obu uruchomieniach.
2. Sprawdź obecność tabel `Products` oraz `__EFMigrationsHistory` w SQLite.
3. Uruchom `dotnet ef migrations list` – oczekiwany wpis `20261115000000_InitialProducts`.
4. Sprawdź `appsettings*.json` – brak haseł i tokenów.
5. W Production bez `ConnectionStrings:Products` start powinien zakończyć się czytelnym błędem.
6. W Production z konfiguracją bazy bez schematu aplikacja **nie tworzy automatycznie tabel**.
7. Sprawdź, że indeks `IX_Products_Sku` jest unikalny. Właściwości `HasPrecision(18,2)` w SQLite opisują model EF; SQLite nie wymusza takiej precyzji liczbowej jak SQL Server.

**Uwagi dydaktyczne:** Nie używaj `EnsureCreated()` razem z migracjami. Nie zapisuj rzeczywistych sekretów do plików śledzonych przez Git. SQLite zastosowano ze względu na prostotę uruchomienia; przy SQL Server należy wygenerować osobny zestaw migracji dostosowany do providera.
