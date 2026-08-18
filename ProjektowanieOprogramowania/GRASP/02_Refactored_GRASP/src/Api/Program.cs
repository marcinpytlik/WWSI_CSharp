using Grasp.Refactored.Application;
using Grasp.Refactored.Contracts;
using Grasp.Refactored.Infrastructure;
using Grasp.Refactored.Ports;
using Grasp.Refactored.Pricing;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IEmailSender, ConsoleEmailSender>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<IPricingStrategyFactory, PricingStrategyFactory>();
builder.Services.AddSingleton<IVatPolicy, Vat23>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderController>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/v1/orders",
        async ([FromBody] CreateOrderDto dto, OrderController controller) => await controller.Create(dto))
    .WithName("CreateOrder_Refactored");

app.MapGet("/api/v1/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program;
