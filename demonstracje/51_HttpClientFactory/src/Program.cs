using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient("quotes", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Quotes:BaseAddress"] ?? "https://quotes.example.invalid/");
    client.Timeout = TimeSpan.FromSeconds(builder.Configuration.GetValue("Quotes:TimeoutSeconds", 2));
});

var app = builder.Build();

app.MapGet("/api/v1/quote", async (IHttpClientFactory factory, CancellationToken cancellationToken) =>
{
    var http = factory.CreateClient("quotes");
    var quote = await http.GetFromJsonAsync<QuoteDto>("/quote", cancellationToken);
    return quote is null ? Results.StatusCode(StatusCodes.Status502BadGateway) : Results.Ok(quote);
});

app.Run();

public sealed record QuoteDto(string Text, string Author);
public partial class Program;
