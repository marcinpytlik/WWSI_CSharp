# GoF — 24 wzorce projektowe w .NET 10 (C#)

Repozytorium zawiera **24** foldery ze wzorcami (23 oryginalne GoF + bonus *Null Object*).
Każdy folder ma projekt `src` oraz testy `tests` (xUnit).

Przykłady są **minimalne** — to kompilowalna ściąga, nie pełna aplikacja.
Kiedy stosować (i kiedy nie), jest w [GoF_24_Wzorce_CheatSheet.md](../GoF_24_Wzorce_CheatSheet.md).

## Wymagania

- .NET SDK 10.x
- VS Code / Visual Studio / Rider

## Komendy (z korzenia repo)

```bash
dotnet test WWSI_CSharp_NET10.sln
```

Albo wejdź do konkretnego folderu wzorca i uruchom `dotnet test`.

## Struktura folderu

```
01_Singleton/
  src/01_Singleton.csproj
  src/01_SingletonDemo.cs
  tests/01_Singleton.Tests.csproj
  tests/01_SingletonTests.cs
```

## Kiedy nie używać (skrót)

- **Singleton** — nie do stanu użytkownika; w ASP.NET preferuj DI (`AddSingleton`).
- **Interpreter** — nie do pełnego języka; parser + AST albo gotowy silnik.
- **Visitor** — gdy hierarchie często się zmieniają, double dispatch boli; rozważ pattern matching.
- **Flyweight** — dopiero przy mierzalnym koszcie pamięci, nie „na zapas”.
