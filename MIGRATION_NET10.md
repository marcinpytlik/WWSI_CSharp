# Migracja repozytorium do .NET 10

Repozytorium jest ujednolicone do **.NET 10 / C# 14**.

## Zakres

- wszystkie projekty SDK-style: `net10.0`
- `global.json` pinuje SDK 10.x
- `Directory.Build.props` + `Directory.Packages.props` (Central Package Management)
- materiały laboratoryjne i polecenia `dotnet new` wskazują `net10.0`
- ASP.NET Core: `Microsoft.AspNetCore.Mvc.Testing` 10.0.10
- Swagger: `Swashbuckle.AspNetCore` 10.2.3
- solution laboratoriów semestru 4: `LinqDotnet10Challenges.sln`

## Wymagania

.NET 10 SDK: <https://dotnet.microsoft.com/download>

## Weryfikacja

```bash
dotnet --info
dotnet restore WWSI_CSharp_NET10.sln
dotnet build WWSI_CSharp_NET10.sln
dotnet test WWSI_CSharp_NET10.sln --no-build
```

Laboratoria semestru 4:

```bash
dotnet test semestr4/labs/LinqDotnet10Challenges.sln
```

## Solution

`WWSI_CSharp_NET10.sln` obejmuje wszystkie projekty w repozytorium.
Legacy `WWSI_CSharp.sln` (martwe ścieżki) została usunięta.
