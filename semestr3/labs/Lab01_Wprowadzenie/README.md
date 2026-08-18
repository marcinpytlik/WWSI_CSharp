# Lab01 - Wprowadzenie (net8.0)

Czas: 90-120 min | Poziom: poczatkujacy -> sredniozaawansowany

## Start
```pwsh
cd Lab01_Wprowadzenie\src
dotnet restore
dotnet build
dotnet test
```

## Uruchamianie
```pwsh
dotnet run --project src -- --help
dotnet run --project src -- greet SQLManiak
dotnet run --project src -- add 2 3
"4 5" | dotnet run --project src -- add
"hello" | dotnet run --project src -- to-json
```

## VS Code
- Ctrl+Shift+B -> build
- Terminal -> Run Task… -> test/format
- Debug: konfiguracja ".NET Launch (src)"

## Jakosc
```pwsh
dotnet format
dotnet test --collect:"XPlat Code Coverage"
```

## Struktura
```
/Lab01_Wprowadzenie/
  README.md
  .editorconfig
  .gitignore
  src/
    Program.cs
    src.csproj
  tests/
    SampleTests.cs
    tests.csproj
  .vscode/
    launch.json
    tasks.json
```

## Commit
```pwsh
git add .
git commit -m "Finish Lab01 Wprowadzenie"
```
