namespace Demo01;

public static class Calculator
{
    public static double Compute(string op, double a, double b) => op switch
    {
        "add" => a + b,
        "sub" => a - b,
        "mul" => a * b,
        "div" when b != 0 => a / b,
        "div" => throw new DivideByZeroException("Cannot divide by zero."),
        _ => throw new ArgumentException($"Unknown operation: {op}", nameof(op))
    };
}

public static class App
{
    public static int Run(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help")
        {
            output.WriteLine("Usage: add|sub|mul|div <a> <b>");
            return 0;
        }

        if (args.Length != 3
            || !double.TryParse(args[1], out var a)
            || !double.TryParse(args[2], out var b))
        {
            error.WriteLine("Expected: <op> <number> <number>");
            return 1;
        }

        try
        {
            output.WriteLine(Calculator.Compute(args[0], a, b));
            return 0;
        }
        catch (Exception ex)
        {
            error.WriteLine($"ERROR: {ex.Message}");
            return 2;
        }
    }

    public static int Main(string[] args) => Run(args, Console.Out, Console.Error);
}
