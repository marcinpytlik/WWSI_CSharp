using System.Globalization;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
while (true)
{
    Console.WriteLine("\n=== Semestr 3 / Tydzień 5: klasy i obiekty ===");
    Console.WriteLine("1. Person  2. Product  3. BankAccount  4. Library  5. Order  0. Koniec");
    Console.Write("Wybierz ćwiczenie: ");
    switch (Console.ReadLine()?.Trim())
    {
        case "1":
            var anna = new Person("Anna", 2000);
            var jan = new Person("Jan", 1995);
            Console.WriteLine($"{anna.Name}: {anna.GetAge(DateTime.Today.Year)} lat");
            Console.WriteLine($"{jan.Name}: {jan.GetAge(DateTime.Today.Year)} lat");
            break;
        case "2":
            var laptop = new Product("Laptop", 4200m);
            Console.WriteLine($"{laptop.Name}: {laptop.GetPriceAfterDiscount(15m):0.00} zł po rabacie 15%");
            TryShow(() => laptop.GetPriceAfterDiscount(120m));
            break;
        case "3":
            var account = new BankAccount("PL-DEMO-001");
            account.Deposit(500m);
            account.Withdraw(125m);
            Console.WriteLine($"Saldo: {account.Balance:0.00} zł");
            TryShow(() => account.Withdraw(1000m));
            break;
        case "4":
            var library = new Library();
            library.Add(new Book("C# od podstaw", "A. Autor"));
            library.Add(new Book("SQL Server", "B. Autor"));
            Console.WriteLine($"Znaleziono: {library.FindByTitle("sql")?.Title ?? "brak"}");
            Console.WriteLine($"Usunięto: {library.RemoveByTitle("SQL Server")}");
            Console.WriteLine($"Pozostało książek: {library.Books.Count}");
            break;
        case "5":
            var order = new Order(1001);
            order.AddItem(new Product("Klawiatura", 120m), 2);
            order.AddItem(new Product("Mysz", 75m), 1);
            Console.WriteLine(order);
            break;
        case "0": return;
        default: Console.WriteLine("Wybierz liczbę od 0 do 5."); break;
    }
}

static void TryShow(Action action)
{
    try { action(); }
    catch (ArgumentException ex) { Console.WriteLine($"Odrzucono niepoprawną operację: {ex.Message}"); }
    catch (InvalidOperationException ex) { Console.WriteLine($"Odrzucono niepoprawną operację: {ex.Message}"); }
}

public sealed class Person
{
    public string Name { get; }
    public int BirthYear { get; }
    public Person(string name, int birthYear)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Imię jest wymagane.", nameof(name));
        if (birthYear < 1900 || birthYear > DateTime.Today.Year)
            throw new ArgumentOutOfRangeException(nameof(birthYear));
        Name = name.Trim();
        BirthYear = birthYear;
    }
    public int GetAge(int currentYear)
    {
        if (currentYear < BirthYear) throw new ArgumentOutOfRangeException(nameof(currentYear));
        return currentYear - BirthYear; // Przybliżenie: znamy rok, a nie dokładną datę urodzenia.
    }
}

public sealed class Product
{
    public string Name { get; }
    public decimal Price { get; }
    public Product(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nazwa jest wymagana.", nameof(name));
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price));
        Name = name.Trim();
        Price = price;
    }
    public decimal GetPriceAfterDiscount(decimal percent)
    {
        if (percent is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(percent));
        return decimal.Round(Price * (1 - percent / 100m), 2, MidpointRounding.AwayFromZero);
    }
}

public sealed class BankAccount
{
    private decimal _balance;
    public string Number { get; }
    public decimal Balance => _balance;
    public BankAccount(string number)
    {
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Numer konta jest wymagany.", nameof(number));
        Number = number;
    }
    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        _balance += amount;
    }
    public void Withdraw(decimal amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (amount > _balance) throw new InvalidOperationException("Niewystarczające środki.");
        _balance -= amount;
    }
}

public sealed record Book(string Title, string Author);

public sealed class Library
{
    private readonly List<Book> _books = [];
    public IReadOnlyList<Book> Books => _books.AsReadOnly();
    public void Add(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);
        if (string.IsNullOrWhiteSpace(book.Title)) throw new ArgumentException("Tytuł jest wymagany.", nameof(book));
        _books.Add(book);
    }
    public Book? FindByTitle(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase)) return null;
        return _books.FirstOrDefault(b => b.Title.Contains(phrase, StringComparison.OrdinalIgnoreCase));
    }
    public bool RemoveByTitle(string title)
    {
        var found = _books.FirstOrDefault(b => string.Equals(b.Title, title, StringComparison.OrdinalIgnoreCase));
        return found is not null && _books.Remove(found);
    }
}

public sealed class OrderItem
{
    public Product Product { get; }
    public int Quantity { get; }
    public decimal LineTotal => Product.Price * Quantity;
    public OrderItem(Product product, int quantity)
    {
        Product = product ?? throw new ArgumentNullException(nameof(product));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        Quantity = quantity;
    }
}

public sealed class Order
{
    private readonly List<OrderItem> _items = [];
    public int Number { get; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
    public decimal Total => _items.Sum(item => item.LineTotal);
    public Order(int number)
    {
        if (number <= 0) throw new ArgumentOutOfRangeException(nameof(number));
        Number = number;
    }
    public void AddItem(Product product, int quantity) => _items.Add(new OrderItem(product, quantity));
    public override string ToString()
    {
        var lines = _items.Select(i => $"  {i.Product.Name} × {i.Quantity} = {i.LineTotal:0.00} zł");
        return $"Zamówienie #{Number}\n{string.Join(Environment.NewLine, lines)}\nRAZEM: {Total:0.00} zł";
    }
}
