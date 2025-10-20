using System;
using System.IO;
using System.Text.Json;

namespace Lab01
{
    public static class Calculator
    {
        public static int Add(int a, int b) => checked(a + b);
        public static int ParseInt(string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentException("Value is null or empty.", nameof(s));
            if (!int.TryParse(s, out var val))
                throw new FormatException($"'{s}' is not a valid integer.");
            return val;
        }
    }

    public class App
    {
        public static int Run(string[] args, TextReader input, TextWriter output, TextWriter error)
        {
            if (args.Length == 0 || args[0] is "--help" or "-h")
            {
                output.WriteLine(HelpText);
                return 0;
            }

            try
            {
                switch (args[0])
                {
                    case "greet":
                        var name = args.Length > 1 ? args[1] : "World";
                        output.WriteLine($"Hello, {name}!");
                        return 0;

                    case "add":
                        if (args.Length >= 3)
                        {
                            var a = Calculator.ParseInt(args[1]);
                            var b = Calculator.ParseInt(args[2]);
                            output.WriteLine(Calculator.Add(a, b));
                            return 0;
                        }
                        else
                        {
                            var line = input.ReadLine();
                            if (line is null) throw new InvalidOperationException("No input provided.");
                            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length < 2) throw new InvalidOperationException("Provide two integers.");
                            var a = Calculator.ParseInt(parts[0]);
                            var b = Calculator.ParseInt(parts[1]);
                            output.WriteLine(Calculator.Add(a, b));
                            return 0;
                        }

                    case "to-json":
                        var text = input.ReadLine() ?? string.Empty;
                        var payload = new { input = text, length = text.Length, timestamp = DateTimeOffset.UtcNow };
                        output.WriteLine(JsonSerializer.Serialize(payload));
                        return 0;

                    default:
                        error.WriteLine($"Unknown command: {args[0]}");
                        error.WriteLine(HelpShort);
                        return 1;
                }
            }
            catch (Exception ex)
            {
                error.WriteLine($"ERROR: {ex.GetType().Name}: {ex.Message}");
                return 2;
            }
        }

        public const string HelpShort = "Usage: dotnet run --project src -- [greet [name] | add <a> <b> | to-json | --help]";
        public static readonly string HelpText = @"Lab01 - Wprowadzenie (net8.0)

Usage:
  dotnet run --project src -- greet [name]
  dotnet run --project src -- add <a> <b>
  echo ""2 3"" | dotnet run --project src -- add
  echo ""hello"" | dotnet run --project src -- to-json
  dotnet run --project src -- --help

Commands:
  greet     Wypisuje powitanie (domyslnie 'World')
  add       Dodaje dwie liczby (z argumentow lub stdin)
  to-json   Czyta linie ze stdin i zwraca JSON

Exit codes: 0=OK, 1=usage error, 2=runtime error";
        public static int Main(string[] args) => Run(args, Console.In, Console.Out, Console.Error);
    }
}
