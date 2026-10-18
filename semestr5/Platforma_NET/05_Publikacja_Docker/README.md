# 05 — Publish ASP.NET Core i Docker

Z katalogu głównego repo:
```powershell
dotnet publish semestr5/Platforma_NET/01_Pipeline_DI/src/PipelineDemo.csproj -c Release -o ./.artifacts/api
dotnet ./.artifacts/api/PipelineDemo.dll --urls http://localhost:5000
```
Dla kontenera (Docker Desktop w trybie Linux containers):
```powershell
docker build -f semestr5/Platforma_NET/05_Publikacja_Docker/Dockerfile -t wwsi-pipeline .
docker run --rm -p 8080:8080 wwsi-pipeline
Invoke-RestMethod http://localhost:8080/health
```
Wielostopniowy Dockerfile wykorzystuje SDK do kompilacji, a obraz `aspnet:10.0` do uruchomienia. W środowisku produkcyjnym konfiguruje się też bezpieczeństwo, health checks, logowanie i zmienne środowiskowe.

**Pytania:** czym różni się obraz SDK od ASP.NET runtime? czym się różni publish od build?
