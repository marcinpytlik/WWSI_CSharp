# 01 — Od C# przez IL do CPU

Z korzenia repo:
```powershell
$proj = 'semestr3/Platforma_NET/01_Kompilacja_IL_JIT/src/IlDemo.csproj'
dotnet build $proj -c Release
dotnet run --project $proj -c Release --no-build
Get-ChildItem semestr3/Platforma_NET/01_Kompilacja_IL_JIT/src/bin/Release/net10.0
```

Otwórz `IlDemo.dll` w ILSpy i wybierz `Calculator.Add` → **IL**. Porównaj źródło C#, IL i efekt uruchomienia. `.dll` w tej konfiguracji nie jest automatycznie uniwersalnym kodem maszynowym; wykonanie w typowym runtime wykorzystuje JIT. `dotnet build` kompiluje, `dotnet run` buduje (jeśli bez `--no-build`) i uruchamia.

**Pytania:** Dlaczego widzimy DLL także dla aplikacji konsolowej? Co robi runtimeconfig.json? Czy JIT i Native AOT są tym samym?

**Bez ILSpy:** pokaż DLL, rozmiar plików i plik `.runtimeconfig.json`; nie twierdź, że tekstowy odczyt DLL pokazuje IL.
