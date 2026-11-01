using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CatalogService>();
var app = builder.Build();

var api = app.MapGroup("/api");
var v1 = api.MapGroup("/v1");
var users = v1.MapGroup("/users").WithTags("Users");
var tasks = v1.MapGroup("/tasks").WithTags("Tasks");

// Ćwiczenie 1: /users/{id:int}, 200 / 400 / 404.
users.MapGet("/{id:int}", Results<Ok<UserDto>, BadRequest<string>, NotFound<string>>
    (int id, CatalogService catalog) =>
{
    if (id <= 0) return TypedResults.BadRequest("Id musi być większe od zera.");
    var user = catalog.Users.FirstOrDefault(x => x.Id == id);
    return user is null
        ? TypedResults.NotFound("Nie znaleziono użytkownika.")
        : TypedResults.Ok(user);
});

// Ćwiczenie 2: search, page i pageSize z query string.
users.MapGet("/", Results<Ok<PagedResponse<UserDto>>, BadRequest<string>>
    (string? search, int? page, int? pageSize, CatalogService catalog) =>
{
    int selectedPage = page ?? 1;
    int selectedSize = pageSize ?? 10;
    if (selectedPage < 1 || selectedSize is < 1 or > 100)
        return TypedResults.BadRequest("page >= 1; pageSize od 1 do 100.");

    IEnumerable<UserDto> query = catalog.Users;
    if (!string.IsNullOrWhiteSpace(search))
        query = query.Where(u => u.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

    var list = query.ToArray();
    long offset = ((long)selectedPage - 1) * selectedSize;
    var items = list.Skip((int)Math.Min(offset, int.MaxValue))
        .Take(selectedSize).ToArray();
    return TypedResults.Ok(new PagedResponse<UserDto>(items, list.Length, selectedPage, selectedSize));
});

// Ćwiczenie 3: odrębne grupy users i tasks.
tasks.MapGet("/", (CatalogService catalog) => TypedResults.Ok(catalog.Tasks));
tasks.MapGet("/{id:int}", Results<Ok<TaskDto>, BadRequest<string>, NotFound<string>>
    (int id, CatalogService catalog) =>
{
    if (id <= 0) return TypedResults.BadRequest("Id musi być dodatnie.");
    var task = catalog.Tasks.FirstOrDefault(t => t.Id == id);
    return task is null ? TypedResults.NotFound("Nie znaleziono zadania.") : TypedResults.Ok(task);
});

// Ćwiczenie 4: jawna unia typów 200, 400, 404 (powyżej).
// Ćwiczenie 5: osobne DTO v1 i v2, ta sama usługa domenowa.
v1.MapGet("/products", (CatalogService catalog) =>
    TypedResults.Ok(catalog.Products.Select(p => new ProductV1(p.Id, p.Name, p.Price)).ToArray()));
api.MapGroup("/v2").MapGet("/products", (CatalogService catalog) =>
    TypedResults.Ok(catalog.Products.Select(p =>
        new ProductV2(p.Id, p.Name, p.Price, p.Currency, p.Category)).ToArray()));

app.Run();

public record UserDto(int Id, string Name);
public record TaskDto(int Id, string Title, bool IsDone);
public record PagedResponse<T>(T[] Items, int Total, int Page, int PageSize);
public record Product(int Id, string Name, decimal Price, string Currency, string Category);
public record ProductV1(int Id, string Name, decimal Price);
public record ProductV2(int Id, string Name, decimal Price, string Currency, string Category);

public sealed class CatalogService
{
    public IReadOnlyList<UserDto> Users { get; } =
        [new(1, "Anna Kowalska"), new(2, "Jan Nowak"), new(3, "Maria Zielińska")];
    public IReadOnlyList<TaskDto> Tasks { get; } =
        [new(1, "Poznać routing", true), new(2, "Napisać Minimal API", false)];
    public IReadOnlyList<Product> Products { get; } =
        [new(1, "Monitor", 899.99m, "PLN", "Elektronika"),
         new(2, "Klawiatura", 149.90m, "PLN", "Akcesoria")];
}
