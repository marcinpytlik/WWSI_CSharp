namespace Demo15;

public sealed class FakeConnection : IDisposable
{
    public bool IsOpen { get; private set; } = true;
    public bool Disposed { get; private set; }

    public void Execute(string sql)
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        if (!IsOpen) throw new InvalidOperationException("Connection closed.");
        LastSql = sql;
    }

    public string? LastSql { get; private set; }

    public void Dispose()
    {
        IsOpen = false;
        Disposed = true;
    }
}

public static class ConnectionRunner
{
    public static string RunWithUsing(string sql)
    {
        using var conn = new FakeConnection();
        conn.Execute(sql);
        return conn.LastSql!;
    }

    public static FakeConnection RunAndLeak(string sql)
    {
        var conn = new FakeConnection();
        conn.Execute(sql);
        return conn;
    }
}

public static class Program
{
    public static int Main()
    {
        Console.WriteLine(ConnectionRunner.RunWithUsing("SELECT 1"));
        return 0;
    }
}
