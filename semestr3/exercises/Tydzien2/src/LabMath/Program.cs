using System;
using System.Linq;

namespace LabMath
{
    public static class MathUtils
    {
        public static long Factorial(int n)
        {
            if (n < 0) throw new ArgumentException("Silnia nie jest zdefiniowana dla liczb ujemnych.", nameof(n));
            if (n == 0 || n == 1) return 1;
            return checked(n * Factorial(n - 1));
        }

        public static bool IsPrime(int n)
        {
            if (n < 2) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;
            int limit = (int)Math.Sqrt(n);
            for (int i = 3; i <= limit; i += 2)
                if (n % i == 0) return false;
            return true;
        }

        public static double AverageOfTen(int[] values)
        {
            if (values is null) throw new ArgumentNullException(nameof(values));
            if (values.Length != 10) throw new ArgumentException("Tablica musi miec dokladnie 10 elementow.", nameof(values));
            return values.Average();
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Przyklady:");
            Console.WriteLine($"5! = {MathUtils.Factorial(5)}");
            Console.WriteLine($"IsPrime(13) = {MathUtils.IsPrime(13)}");
            Console.WriteLine($"Average(2..20 step 2) = {MathUtils.AverageOfTen(new[]{2,4,6,8,10,12,14,16,18,20})}");
        }
    }
}
