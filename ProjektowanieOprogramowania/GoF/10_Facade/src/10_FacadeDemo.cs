namespace GoF.Facade;
public class PaymentFacade
{
    public Task<bool> PayAsync(decimal amount) => Task.FromResult(amount > 0);
}
