# 💡 C# – Odpowiedzi i wskazówki

1. **class** – typ referencyjny (na stercie), przekazywany przez referencję; **struct** – typ wartościowy (najczęściej na stosie), kopiowany przy przekazaniu.  
2. Typy wartościowe przechowują dane bezpośrednio (`int`, `bool`, `struct`), referencyjne – adres (`string`, `object`, `class`).  
3. `using Namespace;` – import przestrzeni nazw.  
   `using(var r = new StreamReader(...)) { }` – blok zarządzający cyklem życia `IDisposable`.  
4. `var` = typ znany w kompilacji; `dynamic` = rozstrzygany w runtime (mniej bezpieczny, wolniejszy).  
5. `readonly` – pole ustawiane tylko w konstruktorze; `init` – właściwość tylko podczas inicjalizacji obiektu.  
6. ```csharp
public static class StringExt {
    public static bool IsNullOrWhiteSpaceOrDash(this string s)
        => string.IsNullOrWhiteSpace(s) || s.Trim() == "-";
}
```
7. `==` może być przeciążony; `Equals()` = porównanie semantyczne. Gdy nadpisujesz `Equals`, nadpisz też `GetHashCode`.  
8. `record` to niezmienniczy typ z porównaniem wartościowym; `with` tworzy skopiowany obiekt z modyfikacją.  
9. ```csharp
switch(obj)
{
    case int x when x > 0: Console.WriteLine("plus"); break;
    case string s: Console.WriteLine(s.Length); break;
    default: Console.WriteLine("inne");
}
```
10. Boxing – opakowanie typu wartościowego w `object`; unboxing – odwrotnie. Unikaj przez genericsy i `Span`/`Memory`.

11. ```csharp
var top5 = orders
  .Where(o => o.Date.Year == 2024)
  .GroupBy(o => o.Customer)
  .Select(g => new { Customer = g.Key, Total = g.Sum(o => o.Amount) })
  .OrderByDescending(x => x.Total)
  .Take(5);
```
12. `IEnumerable` – zapytania wykonywane w pamięci; `IQueryable` – kwerendy tłumaczone (np. do SQL) i wykonywane po stronie źródła.  
13. ```csharp
public delegate void ThresholdHandler(int value);
public class Meter {
    public event ThresholdHandler? OnThresholdExceeded;
    public void Add(int v, int threshold) {
        if (v > threshold) OnThresholdExceeded?.Invoke(v);
    }
}
```
14. `await` zwalnia wątek, gdy operacja I/O czeka – brak blokady UI.  
15. `Task.Run` dla CPU-bound; dla I/O-bound używaj natywnego `await` bez dodatkowego wątku.  
16. ```csharp
try { /* ... */ }
catch (IOException ex) when (ex.Message.Contains("full")) { /* filtr */ }
```
17. ```csharp
async Task WorkAsync(CancellationToken ct) {
    for (var i=0;i<1000;i++) {
        ct.ThrowIfCancellationRequested();
        await Task.Delay(10, ct);
    }
}
```
18. Minimalny szkic:
```csharp
public interface IDomainEvent {}
public interface IEventHandler<in T> where T: IDomainEvent { void Handle(T e); }
public class Dispatcher {
    private readonly List<object> _handlers = new();
    public void Register<T>(IEventHandler<T> h) where T: IDomainEvent => _handlers.Add(h);
    public void Publish<T>(T e) where T: IDomainEvent {
        foreach (var h in _handlers.OfType<IEventHandler<T>>()) h.Handle(e);
    }
}
```
Test: zarejestruj mock/spy i aserty na wywołanie.  
19. `IEnumerable` – tylko iteracja; `ICollection` – rozmiar/dodawanie; `IList` – indeksowane; `IReadOnlyList` – tylko odczyt indeksowany. Eksponuj możliwie węższy interfejs.  
20. ```csharp
public readonly struct ValueRange<T> where T: IComparable<T> {
    public T Min { get; }
    public T Max { get; }
    public ValueRange(T min, T max){ Min=min; Max=max; }
    public bool Contains(T v) => v.CompareTo(Min) >= 0 && v.CompareTo(Max) <= 0;
}
```

21. `Span<T>`/`ReadOnlySpan<T>` – przetwarzanie bez alokacji (slice na tablicach, stringach).  
22. `ArrayPool<T>.Shared.Rent()` – pożycz duże bufory na czas operacji i zwróć przez `Return()`.  
23. `Span<T>` – tylko na stosie/stackalloc, nieprzechowywalny w polach; `Memory<T>` – można przechowywać, przenika async.  
24. Reużywaj `HttpClient` (singleton lub `IHttpClientFactory`), ustaw timeouts i polityki ponawiania.  
25. `System.Text.Json` – szybszy, lżejszy; `Newtonsoft.Json` – bogatsze funkcje. Konwerter dla `DateOnly` przez `JsonConverter<DateOnly>`.  
26. ```csharp
[AttributeUsage(AttributeTargets.Method)]
public class AuditAttribute : Attribute {}
// Odczyt:
var hasAudit = methodInfo.GetCustomAttributes(typeof(AuditAttribute), inherit: true).Any();
```
27. Source Generators – generacja kodu przy kompilacji (mapery, serializery). Uwaga na czas kompilacji i złożoność.  
28. ```csharp
public delegate Task Middleware(Context ctx, Func<Task> next);
// Zbuduj łańcuch przez dodawanie delegatów, gdzie każdy woła next().
```
29. `lock` – sekcja krytyczna 1:1; `SemaphoreSlim` – kontrola współbieżności N; `ReaderWriterLockSlim` – wielu czytelników, jeden pisarz.  
30. GC: generacje 0/1/2, LOH (Large Object Heap), tryby `Workstation`/`Server`/`SustainedLowLatency`. Pomiar: EventCounters, `dotnet-counters`, PerfView.
