using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger(); app.UseSwaggerUI();

// "Baza"
var orders = new List<(Guid Id, string Email, decimal Base, decimal Final, DateTime CreatedUtc)>();

app.MapPost("/orders", ([FromBody] CreateOrderDto dto) =>
{
    // 1) Walidacja
    if (dto is null) return Results.BadRequest(new { error = "Body required" });
    if (string.IsNullOrWhiteSpace(dto.Email) || !Regex.IsMatch(dto.Email, ".+@.+"))
        return Results.BadRequest(new { error = "Invalid email" });
    if (dto.BasePrice <= 0) return Results.BadRequest(new { error = "Invalid price" });

    // 2) Kalkulacja (VIP 10%)
    var final = dto.IsVip ? dto.BasePrice * 0.9m : dto.BasePrice;
    // VAT 23%
    final = Math.Round(final * 1.23m, 2);

    // 3) "Zapis do DB"
    var id = Guid.NewGuid();
    orders.Add((id, dto.Email, dto.BasePrice, final, DateTime.UtcNow));

    // 4) "Wysłanie e-maila"
    Console.WriteLine($"MAIL -> {dto.Email} amount: {final}");

    // 5) Odpowiedź
    return Results.Created($"/orders/{id}", new { id, final });
})
.WithName("CreateOrder_Monolithic")
.WithOpenApi();

app.Run();

public record CreateOrderDto(string Email, decimal BasePrice, bool IsVip);

public partial class Program {}
