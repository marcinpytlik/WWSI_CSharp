namespace Oop.AbstractTemplate;

public abstract class Importer
{
    public string Run()
    {
        var raw = Read();
        var parsed = Parse(raw);
        return Save(parsed);
    }
    protected abstract string Read();
    protected abstract string Parse(string raw);
    protected virtual string Save(string data) => data;
}

public class CsvImporter : Importer
{
    protected override string Read() => "a,b";
    protected override string Parse(string raw) => raw.Replace(",", ";");
}
