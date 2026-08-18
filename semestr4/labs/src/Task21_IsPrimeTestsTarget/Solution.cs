namespace Task21_IsPrimeTestsTarget;

public static class Prime
{
    public static bool IsPrime(int n)
    {
        if (n <= 1) return false;
        if (n == 2) return true;
        if (n % 2 == 0) return false;

        var limit = (int)Math.Sqrt(n);
        for (int i = 3; i <= limit; i += 2)
            if (n % i == 0) return false;

        return true;
    }
}
