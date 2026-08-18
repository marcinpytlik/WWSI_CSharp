using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Demo50.Tests;

public class ValidOptionsFactory : WebApplicationFactory<Program>;

public sealed class InvalidOptionsFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder.UseSetting("Smtp:Host", "").UseSetting("Smtp:Port", "0");
}

public class OptionsTests : IClassFixture<ValidOptionsFactory>
{
    private readonly HttpClient _client;

    public OptionsTests(ValidOptionsFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task ValidConfig_ReturnsHost()
    {
        var dto = await _client.GetFromJsonAsync<SmtpDto>("/api/v1/smtp");
        Assert.Equal("smtp.wwsi.edu.pl", dto!.Host);
        Assert.Equal(587, dto.Port);
    }

    private sealed record SmtpDto(string Host, int Port);
}

public class InvalidOptionsTests
{
    [Fact]
    public void InvalidConfig_FailsOnStart()
    {
        using var factory = new InvalidOptionsFactory();
        var ex = Assert.Throws<OptionsValidationException>(() => factory.CreateClient());
        Assert.Contains("SmtpOptions", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
