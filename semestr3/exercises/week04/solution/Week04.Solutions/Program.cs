using System.Globalization;
using System.Text;

namespace Week04.Solutions;

internal static class Program
{
    private static readonly CultureInfo Pl = CultureInfo.GetCultureInfo("pl-PL");

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        while (true)
        {
            Console.WriteLine("\n=== WWSI – C# / Semestr 3 / Tydzień 4 ===");
            Console.WriteLine("1. Biblioteka metod matematycznych (10 pkt)");
            Console.WriteLine("2. Walidator danych z out (15 pkt)");
            Console.WriteLine("3. Przeciążona metoda Format (20 pkt)");
            Console.WriteLine("4. Statystyki tablicy (25 pkt)");
            Console.WriteLine("5. Kalkulator po refaktoryzacji (30 pkt)");
            Console.WriteLine("0. Zakończ");
            Console.Write("Wybierz ćwiczenie: ");
            switch (Console.ReadLine()?.Trim())
            {
                case "1": MathematicalMethods(); break;
                case "2": PositiveNumberValidator(); break;
                case "3": FormattingOverloads(); break;
                case "4": ArrayStatistics(); break;
                case "5": RefactoredCalculator(); break;
                case "0": return;
                default: Console.WriteLine("Wybierz opcję od 0 do 5."); break;
            }
        }
    }

    // Zadanie 1 – 10 pkt: Program steruje wejściem/wyjściem, obliczenia są osobnymi metodami.
    private static void MathematicalMethods()
    {
        decimal a = ReadDecimal("Liczba A: ");
        decimal b = ReadDecimal("Liczba B: ");
        try
        {
            Console.WriteLine($"Dodawanie: {Add(a, b):0.##}");
            Console.WriteLine($"Odejmowanie: {Subtract(a, b):0.##}");
            Console.WriteLine($"Mnożenie: {Multiply(a, b):0.##}");
            Console.WriteLine(b == 0 ? "Dzielenie: niedozwolone (zero)" :
                $"Dzielenie: {Divide(a, b):0.##}");
        }
        catch (OverflowException)
        {
            Console.WriteLine("Przekroczono zakres typu decimal.");
        }
    }

    private static decimal Add(decimal a, decimal b) => a + b;
    private static decimal Subtract(decimal a, decimal b) => a - b;
    private static decimal Multiply(decimal a, decimal b) => a * b;
    private static decimal Divide(decimal a, decimal b) =>
        b == 0 ? throw new DivideByZeroException() : a / b;

    // Zadanie 2 – 15 pkt: out, walidacja w pętli i możliwość ponowienia.
    private static void PositiveNumberValidator()
    {
        if (TryReadPositiveInt("Podaj dodatnią liczbę całkowitą: ", out int value))
            Console.WriteLine($"Odczytano: {value}");
        else
            Console.WriteLine("Brak danych wejściowych (EOF).");
    }

    private static bool TryReadPositiveInt(string prompt, out int value)
    {
        value = 0;
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (input is null) return false;
            if (int.TryParse(input, NumberStyles.Integer, Pl, out int parsed) && parsed > 0)
            {
                value = parsed;
                return true;
            }
            Console.WriteLine("Błąd: wpisz liczbę całkowitą większą od zera. Spróbuj ponownie.");
        }
    }

    // Zadanie 3 – 20 pkt: trzy przeciążenia tej samej metody.
    private static void FormattingOverloads()
    {
        Console.WriteLine(Format("  WWSI C#  "));
        Console.WriteLine(Format(1234567));
        Console.WriteLine(Format(1234.567m));
    }

    private static string Format(string value) => value.Trim();
    private static string Format(int value) => value.ToString("N0", Pl);
    private static string Format(decimal value) => value.ToString("N2", Pl);

    // Zadanie 4 – 25 pkt: jedno wywołanie zwraca cztery statystyki.
    // Dla pustej tablicy nie istnieją minimum, maksimum ani średnia.
    private static void ArrayStatistics()
    {
        Console.Write("Liczby całkowite oddzielone spacją (Enter = pusta tablica): ");
        string input = Console.ReadLine() ?? "";
        string[] tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int[] values = new int[tokens.Length];
        for (int i = 0; i < tokens.Length; i++)
        {
            if (!int.TryParse(tokens[i], NumberStyles.Integer, Pl, out values[i]))
            {
                Console.WriteLine($"Niepoprawna liczba: {tokens[i]}");
                return;
            }
        }

        if (TryGetStatistics(values, out var stats))
        {
            Console.WriteLine($"Min: {stats.Min}; Max: {stats.Max}; Suma: {stats.Sum}; " +
                              $"Średnia: {stats.Average.ToString("N2", Pl)}");
        }
        else
        {
            Console.WriteLine("Tablica jest pusta – nie można obliczyć statystyk.");
        }
    }

    private static bool TryGetStatistics(
        int[] numbers, out (int Min, int Max, long Sum, double Average) stats)
    {
        stats = default;
        if (numbers.Length == 0) return false;

        int min = numbers[0], max = numbers[0];
        long sum = 0;
        foreach (int number in numbers)
        {
            if (number < min) min = number;
            if (number > max) max = number;
            sum += number;
        }
        stats = (min, max, sum, (double)sum / numbers.Length);
        return true;
    }

    // Zadanie 5 – 30 pkt: wejście, walidacja, obliczenia i prezentacja są oddzielone.
    private static void RefactoredCalculator()
    {
        if (!TryReadDecimal("Pierwsza liczba: ", out decimal a)) return;
        if (!TryReadOperator(out char op)) return;
        if (!TryReadDecimal("Druga liczba: ", out decimal b)) return;

        if (TryCalculate(a, b, op, out decimal result, out string error))
            ShowResult(a, b, op, result);
        else
            Console.WriteLine($"Błąd: {error}");
    }

    private static bool TryReadDecimal(string prompt, out decimal value)
    {
        value = default;
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (input is null) return false;
            if (decimal.TryParse(input, NumberStyles.Number, Pl, out value)) return true;
            Console.WriteLine("Niepoprawna liczba. Przykład: 12,50.");
        }
    }

    private static decimal ReadDecimal(string prompt)
    {
        if (TryReadDecimal(prompt, out decimal value)) return value;
        throw new EndOfStreamException("Zakończono wejście.");
    }

    private static bool TryReadOperator(out char op)
    {
        op = default;
        while (true)
        {
            Console.Write("Operator (+, -, *, /): ");
            string? input = Console.ReadLine();
            if (input is null) return false;
            if (input.Length == 1 && input[0] is '+' or '-' or '*' or '/')
            {
                op = input[0];
                return true;
            }
            Console.WriteLine("Dostępne operatory: + - * /.");
        }
    }

    private static bool TryCalculate(
        decimal a, decimal b, char op, out decimal result, out string error)
    {
        result = default;
        error = "";
        if (op == '/' && b == 0)
        {
            error = "Dzielenie przez zero.";
            return false;
        }
        try
        {
            result = op switch
            {
                '+' => Add(a, b),
                '-' => Subtract(a, b),
                '*' => Multiply(a, b),
                '/' => Divide(a, b),
                _ => throw new ArgumentOutOfRangeException(nameof(op))
            };
            return true;
        }
        catch (OverflowException)
        {
            error = "Wynik wykracza poza zakres decimal.";
            return false;
        }
    }

    private static void ShowResult(decimal a, decimal b, char op, decimal result) =>
        Console.WriteLine($"{a.ToString(Pl)} {op} {b.ToString(Pl)} = {result.ToString(Pl)}");
}
