namespace Task25_FileLogger;

public sealed class FileLogger
{
    private readonly string _path;
    private readonly object _lock = new();

    public FileLogger(string path) => _path = path;

    public void Log(string message)
    {
        var line = $"{DateTimeOffset.Now:O} {message}{Environment.NewLine}";
        lock (_lock)
        {
            File.AppendAllText(_path, line);
        }
    }
}
