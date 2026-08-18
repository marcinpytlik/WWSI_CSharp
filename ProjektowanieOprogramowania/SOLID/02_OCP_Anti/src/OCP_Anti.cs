namespace Solid.OCP.Anti;

public class Pricing
{
    public decimal Calc(string type, decimal basePrice)
        => type switch
        {
            "Normal" => basePrice,
            "Vip" => basePrice * 0.9m,
            _ => basePrice
        };
}
