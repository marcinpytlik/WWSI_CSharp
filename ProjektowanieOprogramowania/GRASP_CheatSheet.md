# GRASP — ściąga praktyczna (.NET 9 / C#)

**GRASP (General Responsibility Assignment Software Patterns)** — zestaw zasad nadawania odpowiedzialności klasom/obiektom. Celem jest kod **spójny, luźno powiązany, testowalny i odporny na zmiany**.

W pakiecie: **Information Expert, Creator, Controller, Low Coupling, High Cohesion, Polymorphism, Pure Fabrication, Indirection, Protected Variations**.

> Tip: GRASP uzupełnia SOLID. Myśl: *„komu dać odpowiedzialność?”* zanim dorzucisz nowe klasy i interfejsy.

---

## 1) Information Expert (Ekspert Informacyjny)
**Idea:** Odpowiedzialność daj temu, kto ma **dane/znajomość** potrzebną do jej wykonania.  
**Kiedy:** gdzie policzyć sumę, walidować reguły, zaktualizować stan.
```csharp
public class Order {
    private readonly List<OrderLine> _lines = new();
    public decimal Total() => _lines.Sum(l => l.Price * l.Qty); // Order zna swoje pozycje → naturalne miejsce
}
```
**Antywzorzec:** God-object „Manager” liczący wszystko za wszystkich.

**Pytanie kontrolne:** *Kto ma wiedzę, by zrobić to poprawnie bez nadmiernych zależności?*

---

## 2) Creator (Twórca)
**Idea:** Klasa **A** powinna tworzyć **B**, jeśli B jest jej częścią, A intensywnie używa B, A ma dane do inicjalizacji B.  
**Kiedy:** agregaty (`Order` → `OrderLine`), DTO ↔ encja.
```csharp
public class Order {
    public OrderLine AddLine(string sku, int qty, decimal price) {
        var line = new OrderLine(sku, qty, price); // Creator: Order tworzy swoje linie
        // ... dodaj do kolekcji
        return line;
    }
}
```
**Uwaga:** Nie myl z **Factory Method** (GoF) – tu chodzi o *kontekst odpowiedzialności*, nie o rozszerzalność.

---

## 3) Controller
**Idea:** Jeden obiekt odbiera **zdarzenia zewnętrzne** (UI/HTTP) i deleguje do modelu domeny.  
**Kiedy:** ASP.NET Core: endpoint/handler **nie** robi logiki – jedynie **orchestrates**.
```csharp
// Minimal API
app.MapPost("/orders", async (CreateOrderDto dto, OrderController c) => await c.Create(dto));

public class OrderController {
    private readonly OrderService _svc;
    public OrderController(OrderService svc) => _svc = svc;
    public Task<Guid> Create(CreateOrderDto dto) => _svc.CreateAsync(dto);
}
```
**Antywzorzec:** „Tłusty kontroler” (logika biznesowa w endpointach).

---

## 4) Low Coupling (Niskie sprzężenie)
**Idea:** Minimalne zależności między klasami → mniejsza fala zmian i łatwiejsze testy.  
**Techniki:** DI/IoC, interfejsy, zdarzenia, wydzielanie portów (`IClock`, `IEmailSender`).
```csharp
public class TokenService {
    private readonly IClock _clock; // DIP + Low Coupling
    public TokenService(IClock clock) => _clock = clock;
}
```
**Zapach:** `new` na każdym kroku w logice, wycieki szczegółów bazy/HTTP do warstwy domeny.

---

## 5) High Cohesion (Wysoka spójność)
**Idea:** Klasa/moduł robi **związane** rzeczy. Małe, czytelne odpowiedzialności → SRP wspiera spójność.  
**Objaw:** metody i pola pasują do jednego tematu; brak „dodatków losowych”.
```csharp
public class PriceCalculator {
    public decimal ApplyVat(decimal net, decimal rate) => net * (1 + rate);
    public decimal Discount(decimal price, decimal pct) => price * (1 - pct);
}
```
**Zapach:** klasa, która wie „po trochu o wszystkim”.

---

## 6) Polymorphism (Polimorfizm)
**Idea:** Zamiast `if/switch` po typach — **wymienne implementacje**.  
**Kiedy:** warianty algorytmów/reguł.
```csharp
public interface IPricing { decimal Calc(Order o); }
public class VipPricing : IPricing { public decimal Calc(Order o)=> o.Base * 0.9m; }
// DI wybiera implementację zamiast switcha
```
**Zysk:** OCP; testy per strategia.

---

## 7) Pure Fabrication (Czysta Fabrykacja)
**Idea:** Gdy eksperta nie ma, stwórz **sztuczny typ**, by osiągnąć niskie sprzężenie/wysoką spójność.  
**Kiedy:** logowanie, repozytoria, mapowanie, polityki.
```csharp
public interface IAudit { void Write(string msg); }
public class FileAudit : IAudit { /* zapis do pliku */ }
```
**Uwaga:** To *nie* anemiczny model — to techniczny „pomost” dla separacji odpowiedzialności.

---

## 8) Indirection (Pośrednictwo)
**Idea:** Wstaw pośrednika, aby rozdzielić elementy i obniżyć sprzężenie.  
**Kiedy:** adapter, mediator, event bus, message broker, repozytorium.
```csharp
public interface IMessageBus { Task Publish<T>(T evt); } // klienci nie wiedzą, jak jest dostarczane
```
**Cena:** dodatkowy poziom może utrudniać debug — dokumentuj przepływy.

---

## 9) Protected Variations (Ochrona przed zmianami)
**Idea:** Izoluj miejsca podatne na zmiany za **stabilnymi interfejsami** i hermetyzacją.  
**Kiedy:** zewnętrzne API, formaty plików, dostawcy płatności, *feature flags*.
```csharp
public interface IPayments { Task<bool> Charge(decimal amount); } // implementacje: Stripe/PayPal/Mock
```
**Zysk:** zmiana dostawcy ≠ refaktor całego systemu.

---

## Mini-mapowanie GRASP → ASP.NET Core / EF Core
- **Controller**: endpoint/handler → deleguje do usług domenowych.
- **Information Expert**: metody domeny agregatu (`Order.Total()`), nie w kontrolerze.
- **Low Coupling + DIP**: porty `IClock/IEmail/IBus`; implementacje podsuwane przez DI.
- **High Cohesion**: rozdziel `Domain`, `Application`, `Infrastructure`.
- **Polymorphism**: strategie rabatów, polityki uprawnień.
- **Pure Fabrication**: `Repository`, `UnitOfWork`, `Mapper` (tech obiekty).
- **Indirection**: middleware, pipeline’y, event bus.
- **Protected Variations**: warstwa antykorupcyjna dla zewnętrznych API.

---

## Checklista PR (GRASP Quick Review)
- [ ] Kto jest **ekspertem** tej odpowiedzialności? (Information Expert)  
- [ ] Czy tworzenie obiektów jest blisko ich **użycia**? (Creator)  
- [ ] Czy kontroler tylko **koordynuje**, a nie robi logiki? (Controller)  
- [ ] Czy zależności są **minimalne** i przez **interfejsy**? (Low Coupling + DIP)  
- [ ] Czy klasa jest **spójna tematycznie**? (High Cohesion)  
- [ ] Czy warianty algorytmów to **polimorfizm**, a nie `switch`? (Polymorphism)  
- [ ] Czy utworzyliśmy **pomocniczy typ**, gdy nie było naturalnego eksperta? (Pure Fabrication)  
- [ ] Czy wstawiliśmy **pośrednika** tam, gdzie warto rozdzielić odpowiedzialności? (Indirection)  
- [ ] Czy wrażliwe punkty są **odizolowane interfejsami**? (Protected Variations)

---

## Mikro-ćwiczenie (15–20 min)
Weź endpoint, który robi wszystko: waliduje, liczy ceny, zapisuje do DB i wysyła e-mail.  
1) Wyciągnij **Controller** do koordynacji.  
2) Przenieś logikę do **ekspertów** (agregaty/serwisy).  
3) Wprowadź **strategie** rabatów.  
4) Zastąp zależności **portami** (`IEmail`, `IClock`) i wstrzyknij w DI.  
5) Dodaj testy jednostkowe i integracyjne.

---

> Filozofia: GRASP to soczewka na *odpowiedzialności*. Zanim napiszesz klasę, spytaj: **dlaczego to należy do niej?**
