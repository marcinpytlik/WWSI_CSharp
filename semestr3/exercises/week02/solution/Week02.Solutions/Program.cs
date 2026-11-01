using System.Globalization;
using System.Text;

namespace Week02.Solutions;

internal static class Program
{
    private static readonly CultureInfo Pl = CultureInfo.GetCultureInfo("pl-PL");

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        while (true)
        {
            Console.WriteLine("\n=== WWSI – C# / Tydzień 2 ===");
            Console.WriteLine("1. Typy liczbowe");
            Console.WriteLine("2. Konwerter temperatury");
            Console.WriteLine("3. Bezpieczne parsowanie");
            Console.WriteLine("4. Koszyk zakupowy");
            Console.WriteLine("5. Analiza operatorów");
            Console.WriteLine("0. Zakończ");
            Console.Write("Wybierz ćwiczenie: ");

            string? choice = Console.ReadLine();
            Console.WriteLine();
            switch (choice)
            {
                case "1": NumericTypes(); break;
                case "2": TemperatureConverter(); break;
                case "3": SafeParsing(); break;
                case "4": ShoppingCart(); break;
                case "5": Operators(); break;
                case "0": return;
                default: Console.WriteLine("Wybierz opcję od 0 do 5."); break;
            }
        }
    }

    // Ćwiczenie 1 — 10 pkt.
    private static void NumericTypes()
    {
        int count = 42;
        long population = 8_000_000_000L;
        decimal money = 0.1m + 0.2m;
        double approximation = 0.1 + 0.2;

        Console.WriteLine($"int: {count} – liczba całkowita (32 bity)");
        Console.WriteLine($"long: {population} – liczba całkowita (64 bity)");
        Console.WriteLine($"decimal: 0,1 + 0,2 = {money.ToString(Pl)}");
        Console.WriteLine($"double:  0,1 + 0,2 = {approximation.ToString("G17", Pl)}");
        Console.WriteLine($"decimal == 0,3m: {money == 0.3m}");
        Console.WriteLine($"double == 0,3: {approximation == 0.3}");
        Console.WriteLine("decimal przechowuje ułamki dziesiętne z dużą precyzją;");
        Console.WriteLine("double korzysta z reprezentacji binarnej i może wykazywać błędy zaokrągleń.");
    }

    // Ćwiczenie 2 — 15 pkt.
    private static void TemperatureConverter()
    {
        decimal celsius = ReadDecimal("Temperatura w °C: ", value => value >= -273.15m,
            "Temperatura nie może być niższa od zera bezwzględnego (-273,15°C).");
        decimal fahrenheit = celsius * 9m / 5m + 32m;
        decimal kelvin = celsius + 273.15m;

        Console.WriteLine($"{celsius.ToString("N2", Pl)} °C = {fahrenheit.ToString("N2", Pl)} °F");
        Console.WriteLine($"{celsius.ToString("N2", Pl)} °C = {kelvin.ToString("N2", Pl)} K");
    }

    // Ćwiczenie 3 — 20 pkt.
    private static void SafeParsing()
    {
        Console.Write("Podaj wiek: ");
        string? ageText = Console.ReadLine();
        if (int.TryParse(ageText, out int age) && age >= 0 && age <= 130)
            Console.WriteLine($"Poprawny wiek: {age}");
        else
            Console.WriteLine("Błędny wiek – podaj liczbę całkowitą od 0 do 130.");

        Console.Write("Podaj cenę produktu (np. 19,99): ");
        string? priceText = Console.ReadLine();
        if (decimal.TryParse(priceText, NumberStyles.Number, Pl, out decimal price) && price >= 0)
            Console.WriteLine($"Poprawna cena: {price.ToString("C2", Pl)}");
        else
            Console.WriteLine("Błędna cena – podaj nieujemną liczbę, np. 19,99.");

        Console.WriteLine("Niepoprawne dane zostały obsłużone bez wyjątku.");
    }

    // Ćwiczenie 4 — 25 pkt.
    private static void ShoppingCart()
    {
        decimal unitNet = ReadDecimal("Cena netto za sztukę: ", x => x >= 0,
            "Cena nie może być ujemna.");
        decimal vatPercent = ReadDecimal("Stawka VAT w % (np. 23): ",
            x => x >= 0 && x <= 100, "Stawka musi należeć do przedziału 0–100.");
        int quantity = ReadInt("Liczba sztuk: ", 0, 1_000_000);

        decimal totalNet = unitNet * quantity;
        decimal totalVat = decimal.Round(totalNet * vatPercent / 100m, 2, MidpointRounding.AwayFromZero);
        decimal totalGross = totalNet + totalVat;

        Console.WriteLine($"Netto: {totalNet.ToString("C2", Pl)}");
        Console.WriteLine($"VAT ({vatPercent.ToString("N2", Pl)}%): {totalVat.ToString("C2", Pl)}");
        Console.WriteLine($"Brutto: {totalGross.ToString("C2", Pl)}");
        Console.WriteLine("Przyjęto zaokrąglanie VAT dla całej pozycji/koszyka do 2 miejsc;");
        Console.WriteLine("w rzeczywistym systemie reguły zaokrąglania wynikają z wymagań biznesowych.");
    }

    // Ćwiczenie 5 — 30 pkt.
    private static void Operators()
    {
        int a = 10, b = 3;
        Console.WriteLine("ARYTMETYCZNE:");
        Console.WriteLine($"a + b = {a + b} (dodawanie)");
        Console.WriteLine($"a - b = {a - b} (odejmowanie)");
        Console.WriteLine($"a * b = {a * b} (mnożenie)");
        Console.WriteLine($"a / b = {a / b} (dzielenie całkowite)");
        Console.WriteLine($"a % b = {a % b} (reszta z dzielenia)");

        Console.WriteLine("\nPORÓWNANIA:");
        Console.WriteLine($"a > b = {a > b} (większe)");
        Console.WriteLine($"a == b = {a == b} (równe)");
        Console.WriteLine($"a != b = {a != b} (różne)");

        bool isAdult = true, hasTicket = false;
        Console.WriteLine("\nLOGICZNE:");
        Console.WriteLine($"isAdult && hasTicket = {isAdult && hasTicket} (AND – oba warunki)");
        Console.WriteLine($"isAdult || hasTicket = {isAdult || hasTicket} (OR – dowolny warunek)");
        Console.WriteLine($"!hasTicket = {!hasTicket} (NOT – negacja)");

        Console.WriteLine("\nNULL-COALESCING:");
        string? alias = null;
        Console.WriteLine($"alias ?? \"Gość\" = {alias ?? "Gość"} (wartość zastępcza bez przypisania)");
        alias ??= "Student";
        Console.WriteLine($"alias ??= \"Student\" -> {alias} (przypisanie tylko dla null)");
        alias ??= "Inna wartość";
        Console.WriteLine($"Ponowne ??= -> {alias} (nie nadpisuje istniejącej wartości)");
    }

    private static decimal ReadDecimal(string prompt, Func<decimal, bool> valid, string message)
    {
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, Pl, out decimal value)
                && valid(value))
                return value;

            Console.WriteLine($"Niepoprawne dane. {message}");
        }
    }

    private static int ReadInt(string prompt, int minimum, int maximum)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value)
                && value >= minimum && value <= maximum)
                return value;

            Console.WriteLine($"Wpisz liczbę całkowitą od {minimum} do {maximum}.");
        }
    }
}
