namespace Solid.DIP.Anti;

public class TokenService
{
    // Zależność na konkret (SystemClock wbudowany w klasę)
    public DateTime Expiry() => DateTime.UtcNow.AddMinutes(30);
}
