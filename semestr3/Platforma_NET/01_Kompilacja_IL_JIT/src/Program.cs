Console.WriteLine($"Wynik Calculator.Add(2, 3): {Calculator.Add(2, 3)}");
Console.WriteLine($"Środowisko: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
Console.WriteLine($"Architektura: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");

public static class Calculator
{
    public static int Add(int a, int b) => a + b;
}
