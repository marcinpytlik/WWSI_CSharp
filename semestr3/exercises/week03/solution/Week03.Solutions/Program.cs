using System.Globalization;
using System.Text;

namespace Week03.Solutions;

internal static class Program
{
    private static readonly CultureInfo Pl = CultureInfo.GetCultureInfo("pl-PL");

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        while (true)
        {
            Console.WriteLine("\n=== WWSI – C# / Tydzień 3 ===");
            Console.WriteLine("1. Ocena z punktów (if/else)");
            Console.WriteLine("2. Kalkulator (switch expression)");
            Console.WriteLine("3. Tabliczka mnożenia (pętle)");
            Console.WriteLine("4. Zgadnij liczbę (while)");
            Console.WriteLine("5. Analizator liczb (warunki)");
            Console.WriteLine("0. Zakończ");
            Console.Write("Wybierz ćwiczenie: ");

            switch (Console.ReadLine())
            {
                case "1": GradeFromPoints(); break;
                case "2": Calculator(); break;
                case "3": MultiplicationTable(); break;
                case "4": GuessTheNumber(); break;
                case "5": NumberAnalyzer(); break;
                case "0": return;
                default: Console.WriteLine("Wybierz opcję od 0 do 5."); break;
            }
        }
    }

    // Ćwiczenie 1 — 10 pkt. Przyjęte progi dydaktyczne: 0–50: 2.0;
    // 51–60: 3.0; 61–70: 3.5; 71–80: 4.0; 81–90: 4.5; 91–100: 5.0.
    private static void GradeFromPoints()
    {
        int points = ReadInt("Wynik (0–100): ", 0, 100);
        double grade;
        if (points <= 50) grade = 2.0;
        else if (points <= 60) grade = 3.0;
        else if (points <= 70) grade = 3.5;
        else if (points <= 80) grade = 4.0;
        else if (points <= 90) grade = 4.5;
        else grade = 5.0;

        Console.WriteLine($"Wynik: {points}/100, ocena: {grade.ToString("F1", Pl)}");
    }

    // Ćwiczenie 2 — 15 pkt.
    private static void Calculator()
    {
        decimal a = ReadDecimal("Pierwsza liczba: ");
        Console.Write("Operator (+, -, *, /): ");
        string? op = Console.ReadLine()?.Trim();
        if (op is not ("+" or "-" or "*" or "/"))
        {
            Console.WriteLine("Nieznany operator.");
            return;
        }

        decimal b = ReadDecimal("Druga liczba: ");
        if (op == "/" && b == 0)
        {
            Console.WriteLine("Nie można dzielić przez zero.");
            return;
        }

        try
        {
            decimal result = op switch
            {
                "+" => a + b,
                "-" => a - b,
                "*" => a * b,
                "/" => a / b,
                _ => throw new InvalidOperationException("Nieobsługiwany operator.")
            };
            Console.WriteLine($"Wynik: {result.ToString(Pl)}");
        }
        catch (OverflowException)
        {
            Console.WriteLine("Wynik wykracza poza zakres typu decimal.");
        }
    }

    // Ćwiczenie 3 — 20 pkt.
    private static void MultiplicationTable()
    {
        Console.Write("     ");
        for (int col = 1; col <= 10; col++)
            Console.Write($"{col,4}");
        Console.WriteLine();
        Console.WriteLine(new string('-', 45));

        for (int row = 1; row <= 10; row++)
        {
            Console.Write($"{row,3} |");
            for (int col = 1; col <= 10; col++)
                Console.Write($"{row * col,4}");
            Console.WriteLine();
        }
    }

    // Ćwiczenie 4 — 25 pkt.
    private static void GuessTheNumber()
    {
        int secret = Random.Shared.Next(1, 101);
        int attempts = 0;
        Console.WriteLine("Zgadnij liczbę od 1 do 100.");
        while (true)
        {
            int guess = ReadInt("Twoja propozycja: ", 1, 100);
            attempts++;
            if (guess < secret) Console.WriteLine("Za mało!");
            else if (guess > secret) Console.WriteLine("Za dużo!");
            else
            {
                Console.WriteLine($"Brawo! Liczba {secret}, liczba prób: {attempts}.");
                break;
            }
        }
    }

    // Ćwiczenie 5 — 30 pkt. Kategorie mogą się nakładać.
    private static void NumberAnalyzer()
    {
        var even = new List<int>();
        var odd = new List<int>();
        var byThree = new List<int>();
        var byFive = new List<int>();

        for (int number = 1; number <= 100; number++)
        {
            if (number % 2 == 0) even.Add(number);
            else odd.Add(number);

            if (number % 3 == 0) byThree.Add(number);
            if (number % 5 == 0) byFive.Add(number);
        }

        PrintCategory("Parzyste", even);
        PrintCategory("Nieparzyste", odd);
        PrintCategory("Podzielne przez 3", byThree);
        PrintCategory("Podzielne przez 5", byFive);
    }

    private static void PrintCategory(string name, List<int> numbers)
    {
        Console.WriteLine($"{name} ({numbers.Count}): {string.Join(", ", numbers)}");
    }

    private static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int number)
                && number >= min && number <= max)
                return number;
            Console.WriteLine($"Podaj liczbę całkowitą od {min} do {max}.");
        }
    }

    private static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, Pl, out decimal value))
                return value;
            Console.WriteLine("Podaj liczbę, np. 12,5.");
        }
    }
}
