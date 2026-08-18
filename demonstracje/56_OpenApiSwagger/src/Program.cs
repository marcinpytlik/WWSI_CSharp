var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/v1/hello", () => Results.Ok(new { message = "hello" }))
    .WithName("GetHello")
    .WithTags("Demo");

app.Run();

public partial class Program;
