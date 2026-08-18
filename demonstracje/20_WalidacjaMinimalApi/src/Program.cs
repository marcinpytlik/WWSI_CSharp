using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/api/v1/products", (CreateProduct dto) =>
{
    var errors = Validate(dto);
    if (errors.Count > 0)
        return Results.ValidationProblem(errors);
    return Results.Created($"/api/v1/products/{dto.Sku}", dto);
});

app.Run();

static Dictionary<string, string[]> Validate(CreateProduct dto)
{
    var results = new List<ValidationResult>();
    var ok = Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);
    if (ok) return [];
    return results
        .SelectMany(r => (r.MemberNames.DefaultIfEmpty(string.Empty)).Select(name => (name, r.ErrorMessage ?? "Invalid")))
        .GroupBy(x => x.name)
        .ToDictionary(g => g.Key, g => g.Select(x => x.Item2).ToArray());
}

public sealed class CreateProduct
{
    [Required, MinLength(3), RegularExpression(@"^SKU-\d+$")]
    public string Sku { get; set; } = "";

    [Required, MinLength(2), MaxLength(80)]
    public string Name { get; set; } = "";

    [Range(0.01, 10_000)]
    public decimal Price { get; set; }
}

public partial class Program;
