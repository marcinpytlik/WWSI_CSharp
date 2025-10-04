# GRASP — mini repo (.NET 9 / Minimal API)

Dwa projekty ilustrujące refaktoryzację zgodnie z GRASP (Information Expert, Controller, Low Coupling, High Cohesion, Polymorphism, Pure Fabrication, Indirection, Protected Variations).

- **01_Anti_MonolithicEndpoint** — jeden endpoint robi wszystko: walidacja, kalkulacja, zapis, e-mail, czas.
- **02_Refactored_GRASP** — rozdział odpowiedzialności: Controller + Domain Experts + Ports/Adapters + Strategy + Repository.

## Wymagania
- .NET SDK 9.0+
- VS Code

## Szybki start
```pwsh
# Anti
cd 01_Anti_MonolithicEndpoint
dotnet build
dotnet test

# Refactored
cd ../02_Refactored_GRASP
dotnet build
dotnet test

# Lub z katalogu repo (dla obu): 
cd ..
dotnet build
dotnet test
```
