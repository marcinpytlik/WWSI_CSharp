# 24 wzorce projektowe (Gang of Four) — .NET 9 / C# — ściąga praktyczna

Podział: **Kreacyjne**, **Strukturalne**, **Czynnościowe**. Każdy wzorzec: krótki opis, kiedy użyć, mikro‑przykład w C# (minimalny, ale kompilowalny po uzupełnieniu przestrzeni nazw).

> Uwaga: w realnych projektach łącz wzorce z DI/IoC (np. `Microsoft.Extensions.DependencyInjection`), logowaniem i testami jednostkowymi.

---

## KREACYJNE

### 1) Singleton
**Idea:** Dokładnie jedna instancja i globalny dostęp.  
**Kiedy:** współdzielone zasoby (cache/in‑mem repo) bez stanu użytkownika.
```csharp
public sealed class AppCache {
    private static readonly Lazy<AppCache> _i = new(() => new AppCache());
    public static AppCache Instance => _i.Value;
    private AppCache() {}
}
```

### 2) Factory Method
**Idea:** Podklasy decydują, jaki obiekt utworzyć.  
**Kiedy:** rodzina produktów z kluczową różnicą implementacyjną.
```csharp
abstract class Report {
    public abstract IExporter CreateExporter();
    public void Export() => CreateExporter().Write();
}
class PdfReport : Report { public override IExporter CreateExporter() => new PdfExporter(); }
interface IExporter { void Write(); }
class PdfExporter : IExporter { public void Write() { /* ... */ } }
```

### 3) Abstract Factory
**Idea:** Tworzy całe **rodziny** spójnych obiektów.  
**Kiedy:** motywy/tematy UI, różne dostawcy (SQL/NoSQL).
```csharp
interface IUiFactory { IButton Button(); ITextBox TextBox(); }
class DarkUiFactory : IUiFactory { public IButton Button()=> new DarkButton(); public ITextBox TextBox()=> new DarkTextBox(); }
interface IButton { } interface ITextBox { }
class DarkButton : IButton { } class DarkTextBox : ITextBox { }
```

### 4) Builder
**Idea:** Składanie obiektów krokami, z różnymi konfiguracjami.  
**Kiedy:** złożone DTO/rekordy konfiguracyjne.
```csharp
class QueryBuilder {
    private string _select="*", _from=""; private string? _where;
    public QueryBuilder From(string t){ _from=t; return this; }
    public QueryBuilder Where(string w){ _where=w; return this; }
    public string Build()=> $"SELECT {_select} FROM {_from}" + (_where is null?"":$" WHERE {_where}");
}
```

### 5) Prototype
**Idea:** Klonowanie istniejących obiektów.  
**Kiedy:** kosztowna inicjalizacja; konfiguracje bazowe.
```csharp
public record EmailTemplate(string Subject, string Body)
{
    public EmailTemplate Clone() => this with { };
}
```

---

## STRUKTURALNE

### 6) Adapter
**Idea:** Łączy niekompatybilne interfejsy.  
**Kiedy:** owijanie legacy/third‑party do naszego API.
```csharp
interface INewPay { void Pay(decimal amount); }
class OldPayService { public void DoPay(double a) { /* ... */ } }
class PayAdapter : INewPay {
    private readonly OldPayService _old = new();
    public void Pay(decimal amount) => _old.DoPay((double)amount);
}
```

### 7) Bridge
**Idea:** Oddziela abstrakcję od implementacji.  
**Kiedy:** kilka wariantów wymiarów (np. kształt × renderowanie).
```csharp
interface IRenderer { void DrawCircle(float x,float y,float r); }
class VectorRenderer : IRenderer { public void DrawCircle(float x,float y,float r){ /* ... */ } }
abstract class Shape { protected readonly IRenderer R; protected Shape(IRenderer r)=>R=r; public abstract void Draw(); }
class Circle : Shape { float x,y,r; public Circle(IRenderer r,float x,float y,float r):base(r){this.x=x;this.y=y;this.r=r;} public override void Draw()=>R.DrawCircle(x,y,r); }
```

### 8) Composite
**Idea:** Drzewa obiektów; traktuj liście i węzły jednakowo.  
**Kiedy:** menu, layout, sceny 3D.
```csharp
interface INode { void Render(); }
class Leaf : INode { public void Render(){ /* ... */ } }
class Group : INode { private readonly List<INode> _children=new(); public void Add(INode n)=>_children.Add(n); public void Render(){ foreach(var c in _children) c.Render(); } }
```

### 9) Decorator
**Idea:** Dodaje zachowanie w locie bez dziedziczenia.  
**Kiedy:** cache, retry, logowanie wokół serwisu.
```csharp
interface IData { Task<string> GetAsync(string k); }
class DataWithCache : IData {
    private readonly IData _inner; private readonly Dictionary<string,string> _cache=new();
    public DataWithCache(IData inner)=>_inner=inner;
    public async Task<string> GetAsync(string k){ if(_cache.TryGetValue(k,out var v)) return v; var r=await _inner.GetAsync(k); return _cache[k]=r; }
}
```

### 10) Facade
**Idea:** Proste API nad złożonym subsystemem.  
**Kiedy:** ujednolicone wejście do modułu.
```csharp
class PaymentFacade {
    public Task<bool> PayAsync(decimal amount){ /* orchestrate gateways, logs, db */ return Task.FromResult(true); }
}
```

### 11) Flyweight
**Idea:** Współdzielenie danych niezmiennych, redukcja pamięci.  
**Kiedy:** wiele małych obiektów o tej samej części stanu.
```csharp
class GlyphFactory {
    private readonly Dictionary<char,Glyph> _cache=new();
    public Glyph Get(char c) => _cache.TryGetValue(c, out var g) ? g : _cache[c]=new Glyph(c);
}
record Glyph(char Char); // intrinsic
```

### 12) Proxy
**Idea:** Zastępca kontrolujący dostęp (leniwa inicjalizacja, zdalne wywołanie, kontrola uprawnień).  
**Kiedy:** remote, cache, authorization.
```csharp
interface IRepo { Task<string> Get(int id); }
class SecureRepoProxy : IRepo {
    private readonly IRepo _inner; private readonly Func<bool> _can;
    public SecureRepoProxy(IRepo inner, Func<bool> can){ _inner=inner; _can=can; }
    public Task<string> Get(int id)=> _can() ? _inner.Get(id) : throw new UnauthorizedAccessException();
}
```

---

## CZYNNOŚCIOWE

### 13) Chain of Responsibility
**Idea:** Łańcuch handlerów; każdy decyduje, czy obsłużyć żądanie.  
**Kiedy:** walidacje, pipeline’y, middleware.
```csharp
abstract class Handler {
    protected Handler? Next;
    public Handler SetNext(Handler next){ Next=next; return next; }
    public virtual string Handle(string req)=> Next?.Handle(req) ?? req;
}
class TrimHandler:Handler{ public override string Handle(string r)=> base.Handle(r.Trim()); }
class LowerHandler:Handler{ public override string Handle(string r)=> base.Handle(r.ToLowerInvariant()); }
```

### 14) Command
**Idea:** Polecenie jako obiekt (undo/redo, kolejki).  
**Kiedy:** operacje odwracalne, buforowane.
```csharp
interface ICommand { void Execute(); void Undo(); }
class AddItem : ICommand { private readonly List<int> _list; private readonly int _v; public AddItem(List<int> l,int v){_list=l;_v=v;} public void Execute()=>_list.Add(_v); public void Undo()=>_list.Remove(_v); }
```

### 15) Interpreter
**Idea:** Prosty język dziedzinowy z parserem/ewaluatorem.  
**Kiedy:** filtry, mini‑reguły.
```csharp
interface IExpr { int Eval(); }
record Num(int V) : IExpr { public int Eval()=>V; }
record Add(IExpr L,IExpr R): IExpr { public int Eval()=> L.Eval()+R.Eval(); }
```

### 16) Iterator
**Idea:** Jednolity dostęp do elementów kolekcji.  
**Kiedy:** własne struktury danych.
```csharp
class Range : IEnumerable<int> {
    public IEnumerator<int> GetEnumerator(){ for(int i=0;i<3;i++) yield return i; }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()=>GetEnumerator();
}
```

### 17) Mediator
**Idea:** Centralny koordynator komunikacji między obiektami.  
**Kiedy:** luźne sprzężenie komponentów UI/modułów.
```csharp
interface IMediator{ void Publish(string topic, object payload); }
class SimpleMediator : IMediator {
    private readonly Dictionary<string,List<Action<object>>> _subs=new();
    public void Subscribe(string t, Action<object> h){ _subs.TryAdd(t,new()); _subs[t].Add(h); }
    public void Publish(string t, object p){ if(_subs.TryGetValue(t, out var hs)) foreach(var h in hs) h(p); }
}
```

### 18) Memento
**Idea:** Zapisywanie i przywracanie stanu bez łamania enkapsulacji.  
**Kiedy:** undo, snapshot.
```csharp
class Editor { string _text=""; public void Type(string t)=>_text+=t; public string Save()=>_text; public void Restore(string m)=>_text=m; }
```

### 19) Observer
**Idea:** Powiadamianie obserwatorów o zmianach.  
**Kiedy:** eventy domenowe, UI.
```csharp
class Topic {
    public event Action<string>? OnMsg;
    public void Publish(string m)=>OnMsg?.Invoke(m);
}
```

### 20) State
**Idea:** Zachowanie zależne od stanu bez rozrostu `if`/`switch`.  
**Kiedy:** automaty stanu (workflow, płatności).
```csharp
interface IState { IState Next(); string Name { get; } }
class Draft: IState { public IState Next()=> new Published(); public string Name=>"Draft"; }
class Published: IState { public IState Next()=> new Archived(); public string Name=>"Published"; }
class Archived: IState { public IState Next()=> this; public string Name=>"Archived"; }
```

### 21) Strategy
**Idea:** Wymienne algorytmy za wspólnym interfejsem.  
**Kiedy:** różne metody sortowania, scoringu.
```csharp
interface IDiscount { decimal Apply(decimal price); }
class Percentage : IDiscount { public decimal Apply(decimal p)=> p*0.9m; }
class NoDiscount : IDiscount { public decimal Apply(decimal p)=> p; }
```

### 22) Template Method
**Idea:** Szkielet algorytmu w klasie bazowej; kroki nadpisywane.  
**Kiedy:** pipeline ze stałą kolejnością kroków.
```csharp
abstract class Importer {
    public void Run(){ var raw=Read(); var parsed=Parse(raw); Save(parsed); }
    protected abstract string Read(); protected abstract object Parse(string raw); protected virtual void Save(object data){ /* ... */ }
}
```

### 23) Visitor
**Idea:** Nowe operacje na strukturach bez zmiany klas elementów.  
**Kiedy:** drzewiaste AST, różne obliczenia.
```csharp
interface INodeV { void Accept(IVisitor v); }
class NumberV : INodeV { public int V; public NumberV(int v)=>V=v; public void Accept(IVisitor v)=>v.Visit(this); }
interface IVisitor { void Visit(NumberV n); }
class PrintVisitor : IVisitor { public void Visit(NumberV n){ /* print */ } }
```

### 24) (Bonus) Null Object *(często dorzucany, nie w oryginalnych 23)*
**Idea:** Obiekt „nic nie robi”, zamiast `null`.  
**Kiedy:** eliminacja if‑ów na `null`.
```csharp
interface ILogger { void Log(string msg); }
class NullLogger : ILogger { public void Log(string msg) { } }
```

---

## Szybki mapping na .NET 9 praktykę
- **DI/IoC:** Abstract Factory / Strategy naturalnie przez `AddTransient/AddSingleton` i `IOptions`.
- **Middleware:** wzorzec **Chain of Responsibility**.
- **Minimal APIs:** **Command** (endpoint‑handlers), **Strategy** (polityki), **Decorator** (owijanie serwisów).
- **EF Core:** **Unit of Work/Repository** (poza GoF), plus **Factory** dla kontekstów w testach.
- **Obsługa zdarzeń:** **Observer/Mediator** w module domenowym.

## Testy
Do każdego wzorca twórz testy **Given/When/Then** w xUnit, mockując współpracowników (np. Moq/NSubstitute).

---

> Tip: Traktuj wzorce jak *słownik technik*, nie dogmat. Najpierw prostota, potem wzorce.
