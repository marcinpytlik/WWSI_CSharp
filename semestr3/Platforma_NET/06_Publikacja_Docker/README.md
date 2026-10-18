# 06 — Publikacja, uruchamianie i kontener

Najpierw pokaż samodzielną publikację **bez Dockera**:
```powershell
$proj = 'semestr3/Platforma_NET/01_Kompilacja_IL_JIT/src/IlDemo.csproj'
dotnet publish $proj -c Release -o ./.artifacts/publish --self-contained false
dotnet ./.artifacts/publish/IlDemo.dll
```

Następnie, jeśli działa Docker Desktop, uruchom tę samą prostą aplikację w kontenerze Linux (bez kopiowania SDK do finalnego obrazu). Z głównego katalogu repo:
```powershell
docker build -f semestr3/Platforma_NET/06_Publikacja_Docker/Dockerfile -t wwsi-il-demo .
docker run --rm wwsi-il-demo
```

Porównaj wynik programu (zwłaszcza `ProcessArchitecture` i `FrameworkDescription`). Gospodarz to Windows, a w kontenerze zwykle działa proces Linuksa (Linux containers w Docker Desktop). Wymaga to zgodności architektury procesora i trybu kontenerów.

**Pytania:** Czym różnią się `build` i `publish`? Jaka jest różnica między obrazem SDK i Runtime? Czy kontener to maszyna wirtualna?
