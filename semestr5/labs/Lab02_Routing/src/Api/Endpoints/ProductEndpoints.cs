using Api.Domain;
using Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProducts(this RouteGroupBuilder api)
    {
        var products = api.MapGroup("/products");

        // GET /api/v1/products?minPrice=&maxPrice=&search=&page=&pageSize=
        products.MapGet("/", async (
            [FromServices] AppDbContext db,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default) =>
        {
            if (page <= 0 || pageSize <= 0) return Results.BadRequest(new { error = "page/pageSize must be > 0" });

            var q = db.Products.AsNoTracking().AsQueryable();
            if (minPrice is not null) q = q.Where(p => p.Price >= minPrice);
            if (maxPrice is not null) q = q.Where(p => p.Price <= maxPrice);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(p => p.Name.Contains(s) || (p.Sku != null && p.Sku.Contains(s)));
            }

            var total = await q.CountAsync(ct);
            var items = await q.OrderBy(p => p.Id)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync(ct);

            return Results.Ok(new { total, page, pageSize, items });
        })
        .WithName("GetProducts")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // GET /api/v1/products/{id:int}
        products.MapGet("/{id:int}", async (int id, AppDbContext db, CancellationToken ct) =>
        {
            var p = await db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            return p is null ? Results.NotFound() : Results.Ok(p);
        })
        .WithName("GetProductById")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // GET /api/v1/products/sku/{sku:regex(...)}
        products.MapGet("/sku/{sku:regex(^[A-Z0-9\\-]{3,20}$)}", async (string sku, AppDbContext db, CancellationToken ct) =>
        {
            var p = await db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Sku == sku, ct);
            return p is null ? Results.NotFound() : Results.Ok(p);
        })
        .WithName("GetProductBySku")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /api/v1/products
        products.MapPost("/", async ([FromBody] ProductDto dto, AppDbContext db, CancellationToken ct) =>
        {
            var errors = Validate(dto);
            if (errors is not null) return Results.ValidationProblem(errors);

            var e = new Product { Name = dto.Name.Trim(), Price = dto.Price, Sku = dto.Sku?.Trim() };
            db.Products.Add(e);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/v1/products/{e.Id}", e);
        })
        .WithName("CreateProduct")
        .Produces<Product>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        // PUT /api/v1/products/{id}
        products.MapPut("/{id:int}", async (int id, [FromBody] ProductDto dto, AppDbContext db, CancellationToken ct) =>
        {
            var errors = Validate(dto);
            if (errors is not null) return Results.ValidationProblem(errors);

            var e = await db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (e is null) return Results.NotFound();
            e.Name = dto.Name.Trim();
            e.Price = dto.Price;
            e.Sku = dto.Sku?.Trim();
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("UpdateProduct")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();

        // DELETE /api/v1/products/{id}
        products.MapDelete("/{id:int}", async (int id, AppDbContext db, CancellationToken ct) =>
        {
            var e = await db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (e is null) return Results.NotFound();
            db.Products.Remove(e);
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteProduct")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return api;
    }

    private static Dictionary<string, string[]>? Validate(ProductDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 2)
            errors["Name"] = new[] { "Name is required, min length 2." };
        if (dto.Price < 0)
            errors["Price"] = new[] { "Price must be >= 0." };
        return errors.Count > 0 ? errors : null;
    }

    public record ProductDto(string Name, decimal Price, string? Sku);
}
