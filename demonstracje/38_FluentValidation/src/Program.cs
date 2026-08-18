using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IValidator<CreateProduct>, CreateProductValidator>();
var app = builder.Build();

app.MapPost("/api/v1/products", async (CreateProduct dto, IValidator<CreateProduct> validator) =>
{
    var result = await validator.ValidateAsync(dto);
    if (!result.IsValid)
    {
        var errors = result.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        return Results.ValidationProblem(errors);
    }

    return Results.Created($"/api/v1/products/{dto.Sku}", dto);
});

app.Run();

public sealed class CreateProduct
{
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}

public sealed class CreateProductValidator : AbstractValidator<CreateProduct>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MinimumLength(3).Matches(@"^SKU-\d+$");
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(80);
        RuleFor(x => x.Price).InclusiveBetween(0.01m, 10_000m);
    }
}

public partial class Program;
