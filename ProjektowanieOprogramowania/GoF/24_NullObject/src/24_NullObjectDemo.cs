namespace GoF.NullObject;

public interface ILogger { void Log(string msg); }
public class NullLogger : ILogger { public void Log(string msg) { } }
public class ConsoleLogger : ILogger { public void Log(string msg) => System.Console.WriteLine(msg); }

public class Worker
{
    private readonly ILogger _log;
    public Worker(ILogger log) => _log = log;
    public void Do() => _log.Log("work");
}
