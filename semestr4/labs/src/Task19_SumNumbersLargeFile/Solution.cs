namespace Task19_SumNumbersLargeFile;

public static class NumberSummer
{
    public static long SumNumbers(string path)
    {
        long sum = 0;
        foreach (var line in File.ReadLines(path))
        {
            if (long.TryParse(line.Trim(), out var n))
                sum += n;
        }
        return sum;
    }
}
