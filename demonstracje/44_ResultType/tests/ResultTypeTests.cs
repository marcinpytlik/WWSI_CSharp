using System.Net;
using System.Net.Http.Json;
using Demo44;
using Xunit;

namespace Demo44.Tests;

public class ResultTypeTests
{
    [Fact]
    public void Withdraw_TooMuch_IsError_NotException()
    {
        var result = TransferService.Withdraw(new Account { Id = 1, Balance = 10 }, 50);
        Assert.False(result.IsSuccess);
        Assert.Equal("Insufficient funds.", result.Error);
    }

    [Fact]
    public void Withdraw_Ok_ReturnsNewBalance()
    {
        var result = TransferService.Withdraw(new Account { Id = 1, Balance = 10 }, 4);
        Assert.True(result.IsSuccess);
        Assert.Equal(6, result.Value!.Balance);
    }
}

public class WithdrawApiTests : IClassFixture<Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public WithdrawApiTests(Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Api_MapsFailTo400()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/accounts/1/withdraw", new { amount = 999 });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task Api_MapsOkTo200()
    {
        var res = await _client.PostAsJsonAsync("/api/v1/accounts/1/withdraw", new { amount = 1 });
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}
