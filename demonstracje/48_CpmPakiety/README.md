# Demo 48 — Central Package Management

Dwie biblioteki (`LibA`, `LibB`) referencją **FluentValidation bez `Version=`**.
Wersja jest jedna, w korzeniu repo: `Directory.Packages.props`.

Repo ma już `ManagePackageVersionsCentrally` w `Directory.Build.props`.

## Co pokazać studentom

1. `PackageReference Include="FluentValidation"` — bez wersji.
2. `Directory.Packages.props`: `<PackageVersion Include="FluentValidation" Version="..." />`.
3. Zmiana wersji w **jednym** pliku dotyczy obu projektów.
4. Anti-pattern: `Version="11.0.0"` w `.csproj` przy CPM → błąd restore (`NU1008`).

```xml
<!-- ŹLE przy CPM -->
<PackageReference Include="FluentValidation" Version="11.0.0" />
```

## Testy

```bash
dotnet test demonstracje/48_CpmPakiety/tests/Demo48_CpmPakiety.Tests.csproj
```
