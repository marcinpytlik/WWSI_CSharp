using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Contracts;
using Api.Domain;
using Api.Infrastructure;

namespace Api.Endpoints
{
    public static class ProductsEndpoints
    {
        public static RouteGroupBuilder MapProducts(this RouteGroupBuilder group)
        {
            var products = group.MapGroup("/products");

            products.MapGet("/", async ([FromServices] AppDbContext db, CancellationToken ct) =>
                {
                    var list = await db.Products.AsNoTracking()
                        .OrderBy(p => p.Id)
                        .Select(p => new ProductReadDto(p.Id, p.Name, p.Price, p.CategoryId))
                        .ToListAsync(ct);
                    return Results.Ok(list);
                })
                .WithName("GetProducts")
                .Produces<IEnumerable<ProductReadDto>>(StatusCodes.Status200OK);

            products.MapGet("/{id:int}", async (int id, AppDbContext db, CancellationToken ct) =>
                {
                    var p = await db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
                    if (p is null) return Results.NotFound();
                    return Results.Ok(new ProductReadDto(p.Id, p.Name, p.Price, p.CategoryId));
                })
                .WithName("GetProductById")
                .Produces<ProductReadDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            products.MapPost("/", async ([FromBody] ProductCreateDto dto, AppDbContext db, CancellationToken ct) =>
                {
                    if (!MiniValidator.TryValidate(dto, out var errors))
                        return Results.ValidationProblem(errors);

                    var exists = await db.Categories.AnyAsync(c => c.Id == dto.CategoryId, ct);
                    if (!exists) return Results.ValidationProblem(new Dictionary<string, string[]> {
                        ["CategoryId"] = new []{ "Category does not exist." }
                    });

                    var entity = new Product
                    {
                        Name = dto.Name.Trim(),
                        Price = dto.Price,
                        CategoryId = dto.CategoryId
                    };
                    db.Products.Add(entity);
                    await db.SaveChangesAsync(ct);
                    return Results.Created($"/api/v1/products/{entity.Id}",
                        new ProductReadDto(entity.Id, entity.Name, entity.Price, entity.CategoryId));
                })
                .WithName("CreateProduct")
                .Produces<ProductReadDto>(StatusCodes.Status201Created)
                .ProducesValidationProblem();

            products.MapPut("/{id:int}", async (int id, [FromBody] ProductUpdateDto dto, AppDbContext db, CancellationToken ct) =>
                {
                    if (!MiniValidator.TryValidate(dto, out var errors))
                        return Results.ValidationProblem(errors);

                    var entity = await db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);
                    if (entity is null) return Results.NotFound();
                    entity.Name = dto.Name.Trim();
                    entity.Price = dto.Price;
                    entity.CategoryId = dto.CategoryId;
                    await db.SaveChangesAsync(ct);
                    return Results.NoContent();
                })
                .WithName("UpdateProduct")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .ProducesValidationProblem();

            products.MapDelete("/{id:int}", async (int id, AppDbContext db, CancellationToken ct) =>
                {
                    var entity = await db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);
                    if (entity is null) return Results.NotFound();
                    db.Products.Remove(entity);
                    await db.SaveChangesAsync(ct);
                    return Results.NoContent();
                })
                .WithName("DeleteProduct")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);

            return group;
        }
    }

    // Minimal validation helper (to avoid adding external packages in lab)
    internal static class MiniValidator
    {
        public static bool TryValidate(object obj, out Dictionary<string, string[]> errors)
        {
            var ctx = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            bool valid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(obj, ctx, results, true);
            errors = results
                .SelectMany(r => r.MemberNames.DefaultIfEmpty(string.Empty).Select(m => new { m, r.ErrorMessage }))
                .GroupBy(x => x.m)
                .ToDictionary(g => g.Key, g => g.Where(x => x.ErrorMessage != null).Select(x => x.ErrorMessage!).ToArray());
            return valid;
        }
    }
}
