using System.Collections.Concurrent;
using Demo44;

var accounts = new ConcurrentDictionary<int, Account>();
accounts[1] = new Account { Id = 1, Balance = 100 };

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/v1/accounts/{id:int}", (int id) =>
    accounts.TryGetValue(id, out var account) ? Results.Ok(account) : Results.NotFound());

app.MapPost("/api/v1/accounts/{id:int}/withdraw", (int id, WithdrawDto dto) =>
{
    if (!accounts.TryGetValue(id, out var account))
        return Results.NotFound();
    var result = TransferService.Withdraw(account, dto.Amount);
    if (!result.IsSuccess)
        return Results.BadRequest(new { error = result.Error });
    accounts[id] = result.Value!;
    return Results.Ok(result.Value);
});

app.Run();

public sealed record WithdrawDto(decimal Amount);
public partial class Program;
