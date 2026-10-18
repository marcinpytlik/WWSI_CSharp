# .NET ShowLab — 12 demonstracji niezależnych od semestru

Praktyczne mini-eksperymenty dla studentów WWSI: **najpierw hipoteza, potem uruchomienie i interpretacja**. .NET 10 / PowerShell. Nie są elementem punktacji semestrów 3–5.

| # | Pokaz | Wymagania | Uruchomienie |
|---|---|---|---|
| 01 | [Wydajność](01_Performance/README.md) | SDK .NET 10 | patrz README |
| 02 | [Pamięć i GC](02_Memory_GC/README.md) | SDK .NET 10 | patrz README |
| 03 | [Asynchroniczność](03_Async_Parallel/README.md) | SDK .NET 10 | patrz README |
| 04 | [Wyścig danych](04_RaceCondition/README.md) | SDK .NET 10 | patrz README |
| 05 | [Minimal API](05_MinimalAPI/README.md) | SDK .NET 10 | patrz README |
| 06 | [EF Core N+1](06_EFCore_NPlusOne/README.md) | Istniejące demo / infrastruktura | patrz README |
| 07 | [Hasła i hashowanie](07_PasswordSecurity/README.md) | Istniejące demo / infrastruktura | patrz README |
| 08 | [Timeout i retry](08_Resilience/README.md) | SDK .NET 10 | patrz README |
| 09 | [Diagnostyka](09_Diagnostics/README.md) | SDK .NET 10 | patrz README |
| 10 | [Windows i Linux](10_CrossPlatform/README.md) | SDK .NET 10 | patrz README |
| 11 | [SignalR](11_SignalR/README.md) | Istniejące demo / infrastruktura | patrz README |
| 12 | [RabbitMQ](12_RabbitMQ/README.md) | Istniejące demo / infrastruktura | patrz README |

**Nowy kod wykonywalny**: 01, 02, 03, 04, 05, 08, 09 (diagnostyka istniejącego procesu) i 10 (Dockerfile). **Integracje** 06, 07, 11, 12 korzystają z istniejącego katalogu `demonstracje/`, żeby nie duplikować działających aplikacji.

## Przykładowy blok 90 minut
Wydajność 15 min, GC 15 min, race condition 10 min, retry 15 min, SignalR 15 min, diagnostyka 15 min, wnioski 5 min.

[Instrukcja prowadzącego](INSTRUCTOR-GUIDE.md) · [Karta obserwacji](STUDENT-WORKSHEET.md).

### Pierwsze uruchomienie
```powershell
dotnet --info
dotnet run --project DotNet_ShowLab/01_Performance/src/Demo.csproj -c Release
dotnet run --project DotNet_ShowLab/02_Memory_GC/src/Demo.csproj -c Release -- transient
dotnet run --project DotNet_ShowLab/04_RaceCondition/src/Demo.csproj -c Release
```

**Ostrzeżenia:** wyniki wydajności zależą od sprzętu/JIT/GC i nie są benchmarkiem produkcyjnym. Pomiary powtarzaj. Docker, SQL Server i RabbitMQ są opcjonalne. Wcześniej przygotuj obrazy oraz sprawdź lokalne porty.
