namespace Task01_EvenNumbers;

public static class EvenNumbers
{
    public static int[] GetEven(int[] numbers)
        => numbers.Where(n => n % 2 == 0).ToArray();
}
