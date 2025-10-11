using System;

namespace HelloDocker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello from .NET 9 running in a container!");
            Console.WriteLine($"Time (UTC): {DateTime.UtcNow:o}");
            Console.WriteLine($"Args: {string.Join(' ', args)}");
        }
    }
}
