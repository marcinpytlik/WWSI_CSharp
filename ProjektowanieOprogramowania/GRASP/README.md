# GRASP — mini repo (.NET 10 / Minimal API)

Dwa projekty ilustrujące refaktoryzację zgodnie z GRASP (Information Expert, Controller,
Low Coupling, High Cohesion, Polymorphism, Pure Fabrication, Indirection, Protected Variations).

- **01_Anti_MonolithicEndpoint** — jeden endpoint robi wszystko: walidacja, kalkulacja, zapis, e-mail, czas.
- **02_Refactored_GRASP** — rozdział odpowiedzialności: Controller + Domain Experts + Ports/Adapters + Strategy + Repository.
  Wycena VIP i VAT jest testowana liczbowo (nie tylko kod HTTP). Strategie wybiera fabryka, nie `IServiceProvider`.

## Wymagania

- .NET SDK 10.x
- VS Code / Visual Studio / Rider

## Szybki start

```bash
cd 01_Anti_MonolithicEndpoint
dotnet test

cd ../02_Refactored_GRASP
dotnet test
```

Albo z korzenia repo: `dotnet test WWSI_CSharp_NET10.sln`.
