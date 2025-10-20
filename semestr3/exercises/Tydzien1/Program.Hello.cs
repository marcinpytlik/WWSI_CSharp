namespace App;

public class HelloApp
{
    public static void Main(string[] args)
    {
        Console.Write("Podaj imię: ");
        var name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Hej, bezimienny bohaterze! 😉");
            return;
        }
        Console.WriteLine($"Cześć, {name}!");
    }
}
