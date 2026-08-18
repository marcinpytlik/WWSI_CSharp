var products = new List<Product>
{
    new(1, "Ada notes"),
    new(2, "Notebook"),
    new(3, "Pen"),
    new(4, "Pencil"),
    new(5, "Sticky notes")
};

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/v1/products", (string? q, int skip = 0, int take = 10) =>
{
    if (skip < 0) return Results.BadRequest(new { error = "skip must be >= 0." });
    if (take is < 1 or > 50) return Results.BadRequest(new { error = "take must be 1..50." });

    IEnumerable<Product> query = products;
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Name.Contains(q.Trim(), StringComparison.OrdinalIgnoreCase));

    var filtered = query.OrderBy(p => p.Id).ToList();
    var items = filtered.Skip(skip).Take(take).ToList();
    return Results.Ok(new PageResult(items, filtered.Count, skip, take));
});

app.Run();

public sealed record Product(int Id, string Name);
public sealed record PageResult(IReadOnlyList<Product> Items, int Total, int Skip, int Take);

public partial class Program;
