namespace Task24_ProductPriceValidation;

public sealed record Product
{
    public int Id { get; }
    public string Name { get; }
    public decimal Price { get; }

    public Product(int id, string name, decimal price)
    {
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
        Id = id;
        Name = name;
        Price = price;
    }
}
