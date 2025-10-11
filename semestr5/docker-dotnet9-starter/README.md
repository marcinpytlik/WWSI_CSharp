# Docker + .NET SDK 9.0 Starter

Ten starter pozwala trzymać kod na dysku (np. `C:\tmp`) i uruchamiać/kompilować go w kontenerze z obrazem `mcr.microsoft.com/dotnet/sdk:9.0`.

## Warianty pracy

### 1) `docker run` (jednorazowo)
```powershell
docker pull mcr.microsoft.com/dotnet/sdk:9.0
docker run --rm -it -v C:\tmp:/workspace -w /workspace mcr.microsoft.com/dotnet/sdk:9.0 pwsh
# W kontenerze:
dotnet --info
dotnet new console -n HelloDocker
dotnet run --project HelloDocker
```

### 2) `docker compose`
Plik `docker-compose.yml` montuje folder roboczy pod `/workspace` i startuje powłokę.
```powershell
docker compose run --rm dotnet pwsh
# W kontenerze:
dotnet restore
dotnet build
dotnet run --project HelloDocker
```

### 3) VS Code – Dev Containers
1. Otwórz folder w VS Code.
2. `Ctrl+Shift+P` → **Dev Containers: Reopen in Container**.
3. Terminal w VS Code to już kontener z SDK 9.0.

## Struktura
- `.devcontainer/devcontainer.json` – definicja środowiska kontenerowego.
- `.vscode/tasks.json` – komendy `dotnet` (build/run).
- `.vscode/launch.json` – konfiguracja debugowania.
- `HelloDocker/` – przykładowy projekt `net9.0`.
- `docker-compose.yml` – usługa `dotnet` z montowaniem kodu.
- `scripts/run-container.ps1` – pomocniczy start kontenera.
- `.gitignore` – podstawowe wykluczenia dla .NET.

## Szybki start
```powershell
# Załóżmy, że wypakowałeś do C:	mp\docker-dotnet9-starter
cd C:	mp\docker-dotnet9-starter

# 1) docker run (skrypt)
.\scriptsun-container.ps1

# albo 2) docker compose
docker compose run --rm dotnet pwsh

# W kontenerze uruchom przykładowy projekt
dotnet run --project HelloDocker
```

## Uwaga (Windows/WSL2)
- Używaj prostych ścieżek bez spacji (np. `C:\tmp`).
- Jeśli masz niestandardowe ustawienia udostępniania dysków, upewnij się, że `C:` jest udostępniony w Docker Desktop.
