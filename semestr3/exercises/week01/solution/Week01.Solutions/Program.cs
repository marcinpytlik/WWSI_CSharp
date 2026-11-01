using System.Globalization;
using System.Runtime.InteropServices;

namespace Week01.Solutions;

internal static class Program
{
    private static readonly CultureInfo PolishCulture = CultureInfo.GetCultureInfo("pl-PL");

    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        while (true)
        {
            Console.WriteLine("\n=== WWSI – C# / Tydzień 1 ===");
            Console.WriteLine("1. Pierwsza aplikacja konsolowa");
            Console.WriteLine("2. Kalkulator danych wejściowych");
            Console.WriteLine("3. Formatowanie danych");
            Console.WriteLine("4. Mini wizytówka CLI");
            Console.WriteLine("5. Analizator środowiska");
            Console.WriteLine("0. Zakończ");
            Console.Write("Wybierz ćwiczenie: ");

            string? choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1": FirstApplication(); break;
                case "2": Calculator(); break;
                case "3": FormatData(); break;
                case "4": BusinessCard(); break;
                case "5": EnvironmentAnalyzer(); break;
                case "0": return;
                default: Console.WriteLine("Nieznana opcja. Wybierz liczbę od 0 do 5."); break;
            }
        }
    }

    // Ćwiczenie 1 (10 pkt): trzy informacje, w tym wersja używanego środowiska .NET.
    private static void FirstApplication()
    {
        string firstName = ReadRequiredText("Podaj imię: ");
        string fieldOfStudy = ReadRequiredText("Podaj kierunek studiów: ");

        Console.WriteLine($"Imię: {firstName}");
        Console.WriteLine($"Kierunek: {fieldOfStudy}");
        Console.WriteLine($"Środowisko uruchomieniowe: {RuntimeInformation.FrameworkDescription}");
    }

    // Ćwiczenie 2 (15 pkt): walidacja danych i ochrona przed dzieleniem przez zero.
    private static void Calculator()
    {
        decimal first = ReadDecimal("Pierwsza liczba: ");
        decimal second = ReadDecimal("Druga liczba: ");

        Console.WriteLine($"Suma: {first + second}");
        Console.WriteLine($"Różnica: {first - second}");
        Console.WriteLine($"Iloczyn: {first * second}");

        if (second == 0)
        {
            Console.WriteLine("Iloraz: nie można dzielić przez zero.");
        }
        else
        {
            Console.WriteLine($"Iloraz: {(first / second).ToString("N2", PolishCulture)}");
        }
    }

    // Ćwiczenie 3 (20 pkt): znamy tylko rok urodzenia, więc obliczamy wiek rocznikowy.
    private static void FormatData()
    {
        string firstName = ReadRequiredText("Imię: ");
        string lastName = ReadRequiredText("Nazwisko: ");
        int currentYear = DateTime.Today.Year;
        int birthYear = ReadInt("Rok urodzenia: ", 1900, currentYear);
        decimal heightCm = ReadDecimalInRange("Wzrost w cm: ", 1, 300);

        int calendarAge = currentYear - birthYear;
        Console.WriteLine($"Osoba: {firstName} {lastName}");
        Console.WriteLine($"Rok urodzenia: {birthYear}");
        Console.WriteLine($"Wiek rocznikowy: {calendarAge} lat");
        Console.WriteLine($"Wzrost: {heightCm.ToString("N1", PolishCulture)} cm");
        Console.WriteLine("Uwaga: dokładny wiek wymaga również dnia i miesiąca urodzenia.");
    }

    // Ćwiczenie 4 (25 pkt): stała oraz zmienne lokalne i czytelny układ.
    private static void BusinessCard()
    {
        const string Header = "WIZYTÓWKA";
        string firstName = ReadRequiredText("Imię: ");
        string lastName = ReadRequiredText("Nazwisko: ");
        string fieldOfStudy = ReadRequiredText("Kierunek studiów: ");

        Console.WriteLine(new string('=', 36));
        Console.WriteLine(Header);
        Console.WriteLine(new string('-', 36));
        Console.WriteLine($"Imię i nazwisko : {firstName} {lastName}");
        Console.WriteLine($"Kierunek        : {fieldOfStudy}");
        Console.WriteLine(new string('=', 36));
    }

    // Ćwiczenie 5 (30 pkt): odczyt parametrów uruchomionego programu.
    private static void EnvironmentAnalyzer()
    {
        Console.WriteLine($"Wersja .NET: {RuntimeInformation.FrameworkDescription}");
        Console.WriteLine("  Środowisko uruchomieniowe, w którym działa aplikacja.");

        Console.WriteLine($"System operacyjny: {RuntimeInformation.OSDescription}");
        Console.WriteLine("  Nazwa i wersja systemu operacyjnego.");

        Console.WriteLine($"Procesory logiczne: {Environment.ProcessorCount}");
        Console.WriteLine("  Liczba procesorów logicznych dostępnych dla procesu.");

        Console.WriteLine($"Katalog bieżący: {Environment.CurrentDirectory}");
        Console.WriteLine("  Katalog roboczy procesu (nie musi być folderem pliku .exe).");
    }

    private static string ReadRequiredText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? value = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            Console.WriteLine("Wartość nie może być pusta.");
        }
    }

    private static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? value = Console.ReadLine();
            if (decimal.TryParse(value, NumberStyles.Number, PolishCulture, out decimal result))
                return result;

            Console.WriteLine("Podaj poprawną liczbę (np. 12,5).");
        }
    }

    private static decimal ReadDecimalInRange(string prompt, decimal minimum, decimal maximum)
    {
        while (true)
        {
            decimal value = ReadDecimal(prompt);
            if (value >= minimum && value <= maximum)
                return value;

            Console.WriteLine($"Wpisz wartość od {minimum} do {maximum}.");
        }
    }

    private static int ReadInt(string prompt, int minimum, int maximum)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int result) &&
                result >= minimum && result <= maximum)
                return result;

            Console.WriteLine($"Podaj liczbę całkowitą od {minimum} do {maximum}.");
        }
    }
}
