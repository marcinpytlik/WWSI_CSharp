# 04 — Jeden ekosystem, różne rodzaje aplikacji

Z głównego katalogu repo uruchom:
```powershell
dotnet run --project demonstracje/01_KalkulatorCli/src -- add 2 3
dotnet run --project demonstracje/07_NotesApi/src
```

Druga komenda uruchamia serwer HTTP, który działa, dopóki go nie zatrzymasz (`Ctrl+C`). Ustal adres/port i ścieżki endpointów z komunikatu aplikacji i [jej README](../../../demonstracje/07_NotesApi/README.md), a następnie przetestuj `Invoke-RestMethod` w drugim terminalu.

Zwróć uwagę: aplikacja konsolowa i ASP.NET Core wykorzystują .NET, ale inny model wykonania i biblioteki. **Nie** trzeba na tych zajęciach implementować CRUD.

**Pytanie:** Jakie inne rodzaje aplikacji można napisać na .NET? Omów desktop, worker, gRPC i rozwiązania mobilne jako przegląd, nie instaluj ich na zajęciach.
