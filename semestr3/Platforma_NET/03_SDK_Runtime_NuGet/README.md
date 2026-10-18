# 03 — SDK, Runtime, NuGet

```powershell
dotnet --info
dotnet --list-sdks
dotnet --list-runtimes
dotnet new list
dotnet list semestr3/Platforma_NET/01_Kompilacja_IL_JIT/src/IlDemo.csproj package
```

Sprawdź `global.json` w katalogu głównym repo oraz `.csproj`. Zależności NuGet obejrzyj w istniejącej demonstracji z pakietami, np. `demonstracje/08_BibliotekaEf`. Dla tej demonstracji projekt IL celowo nie ma zewnętrznych pakietów.

Porównanie (Windows x64; zmień RID dla innej platformy):
```powershell
$proj = 'semestr3/Platforma_NET/01_Kompilacja_IL_JIT/src/IlDemo.csproj'
dotnet publish $proj -c Release -o ./.artifacts/fdd --self-contained false
dotnet publish $proj -c Release -r win-x64 -o ./.artifacts/scd --self-contained true
```

Wariant framework-dependent wymaga odpowiedniego Runtime na komputerze docelowym; self-contained dołącza runtime i zajmuje więcej miejsca. **Uwaga:** self-contained nie oznacza jednego pliku ani przenośności między systemami; zależy od RID. Wyniki trafiają do katalogu `.artifacts` (lokalnego, nie commituj wygenerowanych artefaktów).

**Pytanie:** Czy SDK musi być na serwerze, na którym uruchamiamy aplikację?
