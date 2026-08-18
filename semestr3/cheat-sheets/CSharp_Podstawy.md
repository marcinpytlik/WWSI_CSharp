# Ściąga — C# / .NET 10 (semestr 3)

## CLI

```bash
dotnet new console -f net10.0
dotnet test
dotnet run --project src -- --help
```

## Typy

- wartość (`int`, `struct`, `record struct`) — kopia
- referencja (`class`, `record`) — kopia referencji
- `string?` + `TryParse`, nie `Parse` na inputcie

## OOP

- właściwości zamiast publicznych pól
- `virtual`/`override`; `abstract` gdy nie ma sensu instancja bazowa
- kwadrat **nie** dziedziczy po prostokącie z seterami W/H (LSP)

## Kolekcje

| Potrzeba | Typ |
|---|---|
| kolejność, indeks | `List<T>` |
| klucz → wartość | `Dictionary<TKey,TValue>` |
| unikalność | `HashSet<T>` |
| FIFO / LIFO | `Queue<T>` / `Stack<T>` |

## Wyjątki

- wąski typ; `when` filtr
- nie łap `Exception` żeby „było cicho”

## xUnit

```csharp
[Theory]
[InlineData(2, 2, 4)]
public void Add(int a, int b, int expected) => Assert.Equal(expected, a + b);
```

AAA: Arrange / Act / Assert. Testuj swoją logikę, nie framework.
