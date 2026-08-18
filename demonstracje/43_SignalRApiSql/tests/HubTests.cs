using System.Net.Http.Json;
using Demo43;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demo43.Tests;

public sealed class IsolatedApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        builder.UseSetting("Testing", "true");
        builder.ConfigureServices(services =>
        {
            foreach (var descriptor in services.Where(d =>
                         d.ServiceType == typeof(ChatDb) ||
                         d.ServiceType == typeof(DbContextOptions<ChatDb>)).ToList())
                services.Remove(descriptor);
            services.AddDbContext<ChatDb>(o => o.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Dispose();
        base.Dispose(disposing);
    }
}

public class HubTests : IClassFixture<IsolatedApiFactory>
{
    private readonly IsolatedApiFactory _factory;

    public HubTests(IsolatedApiFactory factory)
        => _factory = factory;

    [Fact]
    public async Task Send_PersistsAndBroadcasts()
    {
        var tcs = new TaskCompletionSource<(int Id, string Text)>(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var connection = new HubConnectionBuilder()
            .WithUrl(new Uri(_factory.Server.BaseAddress!, "/hubs/chat"), o =>
            {
                o.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
            })
            .Build();

        connection.On<int, string, DateTime>("Receive", (id, text, _) => tcs.TrySetResult((id, text)));
        await connection.StartAsync();
        await connection.InvokeAsync("Send", "Hello sala");

        var received = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal("Hello sala", received.Text);

        var client = _factory.CreateClient();
        var list = await client.GetFromJsonAsync<List<ChatMessage>>("/api/v1/messages");
        Assert.Contains(list!, m => m.Text == "Hello sala");
    }
}
