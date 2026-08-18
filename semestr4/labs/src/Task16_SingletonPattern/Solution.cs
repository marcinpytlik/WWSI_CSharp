namespace Task16_SingletonPattern;

public sealed class SingletonLogger
{
    private SingletonLogger() { }

    public static SingletonLogger Instance { get; } = new();

    public List<string> Messages { get; } = new();

    public void Log(string message) => Messages.Add(message);
}
