namespace GoF.Adapter;

public interface INewPay { string Pay(decimal amount); }

public class OldPayService { public string DoPay(double a) => $"OLD:{a:0.00}"; }

public class PayAdapter : INewPay
{
    private readonly OldPayService _old = new();
    public string Pay(decimal amount) => _old.DoPay((double)amount);
}
