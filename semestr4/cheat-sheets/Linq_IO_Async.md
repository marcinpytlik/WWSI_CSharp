# Ściąga — LINQ, IO, async (semestr 4)

## LINQ (method syntax)

```csharp
var q = items
    .Where(x => x.Active)
    .Select(x => x.Name)
    .OrderBy(x => x);
```

- `GroupBy`, `Join`, `Aggregate` — świadomie
- wielokrotna enumeracja: zmaterializuj `ToList()` gdy źródło jest drogie
- `First` rzuca, `FirstOrDefault` nie

## IO

- `StreamReader` / `StreamWriter` w `using`
- JSON: `System.Text.Json` (`JsonSerializer`)
- duże pliki: strumień, nie cały `ReadAllText` bez potrzeby

## Async

```csharp
await Task.WhenAll(tasks);
```

- nie `.Result` / `.Wait()` w ASP.NET
- `CancellationToken` od Lab HttpClient
- wyjątki z `WhenAll`: `AggregateException`

## HttpClient

- jeden klient (DI `IHttpClientFactory` albo pole static/handler w testach)
- testy: fake `HttpMessageHandler`

## Warstwy

UI → Application (serwisy) → Ports (interfejsy) → Infrastructure (pliki, HTTP)
