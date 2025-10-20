using System.Globalization;

namespace App;

public class MathApp
{
    public static void Main(string[] args)
    {
        var ci = CultureInfo.InvariantCulture;

        Console.Write("Podaj pierwszą liczbę: ");
        if (!double.TryParse(Console.ReadLine(), NumberStyles.Float, ci, out var a))
        {
            Console.WriteLine("To nie wygląda na liczbę.");
            return;
        }

        Console.Write("Podaj drugą liczbę: ");
        if (!double.TryParse(Console.ReadLine(), NumberStyles.Float, ci, out var b))
        {
            Console.WriteLine("To nie wygląda na liczbę.");
            return;
        }

        Console.WriteLine($"Suma:        {a + b}");
        Console.WriteLine($"Różnica:     {a - b}");
        Console.WriteLine($"Iloczyn:     {a * b}");
        Console.WriteLine(b == 0
            ? "Iloraz:      nieokreślony (dzielenie przez zero)"
            : $"Iloraz:      {a / b}");
    }
}
