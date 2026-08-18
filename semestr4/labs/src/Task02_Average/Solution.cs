namespace Task02_Average;

public static class AverageCalculator
{
    public static double Average(int[] numbers)
        => numbers.Length == 0 ? throw new ArgumentException("Array is empty.", nameof(numbers))
                               : numbers.Average();
}
