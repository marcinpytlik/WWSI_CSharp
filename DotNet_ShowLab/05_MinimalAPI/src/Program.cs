var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", () => new { message = ".NET ShowLab", time = DateTimeOffset.UtcNow });
app.MapGet("/hello/{name}", (string name) => new { hello = name });
app.MapPost("/echo", (Message message) => Results.Ok(new { received = message.Text, at = DateTimeOffset.UtcNow }));
app.Run();
record Message(string Text);
