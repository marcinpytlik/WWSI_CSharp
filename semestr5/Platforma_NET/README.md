# Platforma .NET w praktyce — semestr 5

Pokaz **co dzieje się z żądaniem HTTP wewnątrz ASP.NET Core**: pipeline middleware → routing → DI → endpoint → odpowiedź, plus EF Core, JWT, telemetry i wdrożenie. Uzupełnienie istniejących laboratoriów, nie ich zamiennik.

## Szybki start
Wymagane .NET 10 SDK (zgodnie z `global.json` repo). Z korzenia repo:
```powershell
dotnet run --project semestr5/Platforma_NET/01_Pipeline_DI/src/PipelineDemo.csproj
```
W drugim terminalu skopiuj adres HTTP z konsoli aplikacji i wykonaj:
```powershell
$base = 'http://localhost:5000' # zmień na rzeczywisty adres wypisany przez aplikację
Invoke-RestMethod "$base/health"
Invoke-RestMethod "$base/orders/42"
Invoke-WebRequest "$base/fail" -SkipHttpErrorCheck # PowerShell 7; dla 5.1 użyj try/catch
```
Dla stałego adresu można uruchomić `dotnet run --project ... --urls http://localhost:5000`.

## Program (90 minut)
| Minuty | Pokaz | Efekt |
|---|---|---|
| 0–20 | [01 Pipeline + DI](01_Pipeline_DI/README.md) | kolejność middleware, scope, nagłówki, 500 |
| 20–35 | [02 EF Core + SQL](02_EF_Core_SQL/README.md) | LINQ → SQL, tracking, transakcje |
| 35–50 | [03 JWT i bezpieczeństwo](03_JWT_Bezpieczenstwo/README.md) | 401 vs 403, uprawnienia |
| 50–65 | [04 Obserwowalność](04_Obserwowalnosc/README.md) | logi, metryki, ślady |
| 65–80 | [05 Publikacja i kontenery](05_Publikacja_Docker/README.md) | publish i runtime |
| 80–90 | dyskusja i [karta obserwacji](STUDENT-WORKSHEET.md) | rozumienie całości |

**[Instrukcja krok po kroku — wszystkie 5 demonstracji](PRZEWODNIK-KROK-PO-KROKU.md)** · [Skrócony scenariusz prowadzącego](INSTRUCTOR-GUIDE.md). Wykorzystujemy istniejące przykłady z katalogu `demonstracje/` bez kopiowania ich implementacji.
