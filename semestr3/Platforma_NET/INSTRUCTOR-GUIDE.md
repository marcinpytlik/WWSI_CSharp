# Scenariusz prowadzącego — 90 minut

Prowadź w terminalu PowerShell z korzenia repo. Najpierw zademonstruj efekt, potem pokaż co za nim stoi. Podawane czasy są orientacyjne.

## Przygotowanie sali
1. `dotnet --info` — sprawdź SDK 10.x oraz runtime; repo zawiera `global.json`.
2. Uruchom obie nowe demonstracje lokalnie i sprawdź, że przyrosty liczników GC mogą różnić się między komputerami.
3. Opcjonalnie przygotuj ILSpy, Docker Desktop i SQL Server; dla braku tych narzędzi są ścieżki zastępcze.
4. Projekty dla semestru 5 używaj do pokazu, a nie do oceniania studentów semestru 3.

## 01: kompilacja → IL → JIT (0–15 min)
- Zapytaj: *czy wynik `dotnet build` to bezpośrednio kod CPU?*
- Wykonaj `dotnet build semestr3/Platforma_NET/01_Kompilacja_IL_JIT/src/IlDemo.csproj -c Release`.
- Pokaż `bin/Release/net10.0/IlDemo.dll` oraz `.runtimeconfig.json`. W ILSpy otwórz DLL i wyświetl IL dla `Calculator.Add`.
- Uruchom `dotnet run --project semestr3/Platforma_NET/01_Kompilacja_IL_JIT/src/IlDemo.csproj -c Release --no-build`.
- Wytłumacz: kompilator C# emituje zwykle IL i metadane; CLR może skompilować metody JIT do kodu natywnego. JIT jest domyślną drogą w tym pokazie, ale .NET obsługuje też AOT.

## 02: zarządzanie pamięcią (15–25 min)
- Uruchom `dotnet run --project semestr3/Platforma_NET/02_CLR_GC_Pamiec/src/GcDemo.csproj -c Release`.
- Obserwuj `GC.CollectionCount`, zużycie pamięci i rezultat `using`.
- Wyjaśnij, że GC zarządza **pamięcią zarządzaną**, a `IDisposable` służy deterministycznemu zwalnianiu zasobów; to nie to samo.
- Nie wyciągaj wniosków wydajnościowych z jednego uruchomienia ani nie prezentuj `GC.Collect()` jako praktyki aplikacyjnej.

## 03: SDK i publikacja (25–35 min)
- Wykonaj `dotnet --list-sdks`, `dotnet --list-runtimes`, `dotnet list package` w projekcie z zależnościami.
- Pokaż `.csproj`, `global.json`; porównaj framework-dependent i self-contained w instrukcji 03.
- SDK służy do budowania; Runtime do uruchamiania aplikacji zależnej od frameworka.

## 04: wszechstronność .NET (35–50 min)
- Pokaz CLI: `dotnet run --project demonstracje/01_KalkulatorCli/src -- add 2 3`.
- Pokaz HTTP: `dotnet run --project demonstracje/07_NotesApi/src`; odczytaj adres nasłuchiwania z konsoli i wyślij GET zgodnie z README projektu.
- Porównaj proces konsolowy i proces serwera HTTP; jeden SDK, różne modele hostowania.

## 05: dostęp do danych (50–65 min)
- Preferuj `demonstracje/08_BibliotekaEf` (SQLite, bez infrastruktury) i pokaż LINQ, kontekst, provider, SQL.
- Jeśli SQL Server jest gotowy, przejdź do `demonstracje/12_EfCoreSqlLeastPrivilege`; wyjaśnij, czym różni się uprawnienie do migracji od prawa aplikacji do zapytań.
- Nie instaluj infrastruktury na żywo — w razie problemów pokaż istniejące README/kod.

## 06: wdrożenie (65–80 min)
- Pokaż `dotnet publish` i katalog wynikowy. Docker jest opcjonalny: opis build i uruchomienia jest w 06.
- Pytanie: czego potrzebuje maszyna docelowa, by wykonać aplikację zależną od środowiska .NET?

## Zakończenie (80–90 min)
- Poproś o uzupełnienie karty obserwacji.
- Pytania: C# vs .NET? IL vs natywny? GC vs Dispose? SDK vs Runtime? Build vs Publish?
- Nie wymagaj od grupy znajomości EF Core, ASP.NET ani Dockera — to panorama możliwości.
