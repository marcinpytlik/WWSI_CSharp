using Demo46;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IXmlQuoteSource, InMemoryXmlQuoteSource>();
builder.Services.AddSingleton<IQuoteClient, XmlQuoteAdapter>();
var app = builder.Build();

app.MapGet("/api/v1/quotes/{id}", async (string id, IQuoteClient client) =>
    await client.GetAsync(id, CancellationToken.None) is { } quote
        ? Results.Ok(quote)
        : Results.NotFound());

app.Run();

public partial class Program;
