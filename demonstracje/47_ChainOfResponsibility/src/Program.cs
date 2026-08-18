using Demo47;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(CodePipeline.Create());
var app = builder.Build();

app.MapPost("/api/v1/codes", (CodeDto dto, CodeHandler pipeline) =>
{
    var error = pipeline.Handle(dto.Code ?? "");
    return error is null
        ? Results.Ok(new { code = dto.Code, status = "ok" })
        : Results.BadRequest(new { error });
});

app.Run();

public sealed record CodeDto(string? Code);
public partial class Program;
