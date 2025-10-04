// Lab03 Sterowanie
// Szkielet aplikacji konsolowej .NET 8
// Tematy do pokrycia w tym labie:
// - if/switch
// - for/while/foreach
// - ćwiczenia z warunków

using System;
using System.CommandLine; // Jeśli dodasz System.CommandLine

namespace LabApp
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("== Lab03 Sterowanie ==");
            Console.WriteLine("Podaj --help aby zobaczyć dostępne opcje (jeśli skonfigurujesz System.CommandLine).");
            // TODO: Zaimplementuj funkcjonalności wymagane w instrukcji labu.
            // TODO: Dodaj osobne klasy/metody w /src zgodnie z zasadą SRP.

            // Przykładowe wywołanie funkcji do zaimplementowania:
            // Console.WriteLine(Calculator.Add(2, 2));

            return 0;
        }
    }

    // Przykładowa klasa do testów
    public static class Calculator
    {
        public static int Add(int a, int b) => a + b;
        public static int Sub(int a, int b) => a - b;
    }
}
