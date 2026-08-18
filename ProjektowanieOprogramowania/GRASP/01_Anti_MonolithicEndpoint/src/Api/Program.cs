using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// Antyprzykład celowy: jeden handler robi walidację, wycenę, zapis, e-mail i odpowiedź.
var orders = new List<(Guid Id, string Email, decimal Base, decimal Final, DateTime CreatedUtc)>();

app.MapPost("/orders", ([FromBody] CreateOrderDto dto) =>
{
    if (dto is null) return Results.BadRequest(new { error = "Body required" });
    if (string.IsNullOrWhiteSpace(dto.Email) || !Regex.IsMatch(dto.Email, ".+@.+"))
        return Results.BadRequest(new { error = "Invalid email" });
    if (dto.BasePrice <= 0) return Results.BadRequest(new { error = "Invalid price" });

    var final = dto.IsVip ? dto.BasePrice * 0.9m : dto.BasePrice;
    final = Math.Round(final * 1.23m, 2);

    var id = Guid.NewGuid();
    orders.Add((id, dto.Email, dto.BasePrice, final, DateTime.UtcNow));
    Console.WriteLine($"MAIL -> {dto.Email} amount: {final}");

    return Results.Created($"/orders/{id}", new { id, final });
})
.WithName("CreateOrder_Monolithic");

app.Run();

public record CreateOrderDto(string Email, decimal BasePrice, bool IsVip);

public partial class Program;
