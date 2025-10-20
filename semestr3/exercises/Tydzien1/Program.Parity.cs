namespace App;

public class ParityApp
{
    public static void Main(string[] args)
    {
        Console.Write("Podaj liczbę całkowitą: ");
        if (!int.TryParse(Console.ReadLine(), out var n))
        {
            Console.WriteLine("To nie jest liczba całkowita.");
            return;
        }

        var isEven = n % 2 == 0;
        Console.WriteLine(isEven ? "Liczba jest parzysta." : "Liczba jest nieparzysta.");
    }
}
