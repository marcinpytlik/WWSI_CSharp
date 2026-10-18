# Platforma .NET w praktyce — grupy 301 i 302

**Cel:** prześledzić co dzieje się od kodu C# do wykonania programu i zobaczyć usługi platformy .NET 10. To pokaz uzupełniający laboratoria semestru 3, **nie** osobny kurs języka.

## Wymagania
- Windows + PowerShell, .NET 10 SDK (zgodnie z `global.json` w repo), VS Code lub Visual Studio;
- opcjonalnie ILSpy, `dotnet-counters`, Docker Desktop i lokalna instancja SQL Server;
- demonstracje 01–03 działają bez Dockera i bez SQL Server.

## Plan pokazu — 90 minut
| Minuty | Demonstracja | Efekt |
|---|---|---|
| 0–15 | [01 Kompilacja, IL, JIT](01_Kompilacja_IL_JIT/README.md) | zobaczyć DLL, IL oraz wykonanie |
| 15–25 | [02 CLR i GC](02_CLR_GC_Pamiec/README.md) | zobaczyć licznik kolekcji GC i Dispose |
| 25–35 | [03 SDK, Runtime, NuGet](03_SDK_Runtime_NuGet/README.md) | rozpoznać narzędzia i modele wdrożenia |
| 35–50 | [04 Rodzaje aplikacji](04_RodzajeAplikacji/README.md) | uruchomić CLI oraz HTTP API |
| 50–65 | [05 Dostęp do danych](05_DostepDoDanych/README.md) | zobaczyć EF Core i bazę |
| 65–80 | [06 Publikacja i Docker](06_Publikacja_Docker/README.md) | porównać build i publish, kontener |
| 80–90 | pytania / obserwacje | podsumować architekturę |

[Instrukcja prowadzącego](INSTRUCTOR-GUIDE.md) · [Karta obserwacji studenta](STUDENT-WORKSHEET.md)

## Szybki start (PowerShell z katalogu głównego repo)
```powershell
dotnet --info
dotnet run --project semestr3/Platforma_NET/01_Kompilacja_IL_JIT/src/IlDemo.csproj -c Release
dotnet run --project semestr3/Platforma_NET/02_CLR_GC_Pamiec/src/GcDemo.csproj -c Release
```

**Uwaga:** projekty pokazowe w tym folderze są celowo małe, a odnośniki do `demonstracje/` wykorzystują istniejące materiały, bez duplikowania kodu. Nie zmieniamy planu, punktacji ani istniejących laboratoriów semestrów 3–5.
