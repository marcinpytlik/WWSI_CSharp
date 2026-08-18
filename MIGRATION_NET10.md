# Migracja repozytorium do .NET 10

Repozytorium zostało ujednolicone do .NET 10.

## Zakres zmian

- wszystkie projekty SDK-style używają `net10.0`,
- materiały laboratoryjne i przykładowe polecenia `dotnet new` wskazują `net10.0`,
- ścieżki uruchomieniowe w materiałach zostały zmienione na `bin/Debug/net10.0`,
- przykłady ASP.NET Core używają `Microsoft.AspNetCore.Mvc.Testing` 10.0.10,
- przykłady Swagger/OpenAPI używają `Swashbuckle.AspNetCore` 10.2.3,
- odniesienia tekstowe do .NET 8/.NET 9 zostały ujednolicone do .NET 10,
- odniesienia do C# 13 zostały zmienione na C# 14,
- solution laboratoriów semestru 4 została przemianowana na `LinqDotnet10Challenges.sln`.

## Wymagania

Do kompilacji wymagany jest zainstalowany .NET 10 SDK.

## Weryfikacja lokalna

```powershell
dotnet --info
dotnet restore
dotnet build .\WWSI_CSharp_NET10.sln
dotnet test .\WWSI_CSharp_NET10.sln --no-build
```

Dla osobnej solution laboratoriów semestru 4:

```powershell
dotnet restore .\semestr4\labs\LinqDotnet10Challenges.sln
dotnet build .\semestr4\labs\LinqDotnet10Challenges.sln
dotnet test .\semestr4\labs\LinqDotnet10Challenges.sln --no-build
```

## Solution główna

Dodano `WWSI_CSharp_NET10.sln` obejmującą wszystkie 154 istniejące projekty. Oryginalna `WWSI_CSharp.sln` została pozostawiona jako legacy, ponieważ już w repo wejściowym wskazywała na 9 nieistniejących projektów.
