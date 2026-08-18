using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOptions<SmtpOptions>()
    .BindConfiguration(SmtpOptions.Section)
    .ValidateDataAnnotations()
    .ValidateOnStart();

var app = builder.Build();

app.MapGet("/api/v1/smtp", (IOptions<SmtpOptions> options) =>
    Results.Ok(new { options.Value.Host, options.Value.Port }));

app.Run();

public sealed class SmtpOptions
{
    public const string Section = "Smtp";

    [Required, MinLength(3)]
    public string Host { get; set; } = "";

    [Range(1, 65535)]
    public int Port { get; set; }
}

public partial class Program;
