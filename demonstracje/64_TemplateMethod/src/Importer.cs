using System.Text.Json;

namespace Demo64;

public sealed record Row(string Sku, int Qty);

public abstract class FileImporter
{
    public IReadOnlyList<Row> Import(string content)
    {
        var rows = Parse(content);
        return rows.Where(Validate).Select(Normalize).ToList();
    }

    protected abstract IEnumerable<Row> Parse(string content);

    protected virtual bool Validate(Row row) => row.Qty > 0 && !string.IsNullOrWhiteSpace(row.Sku);

    protected virtual Row Normalize(Row row) => row with { Sku = row.Sku.Trim().ToUpperInvariant() };
}

public sealed class CsvImporter : FileImporter
{
    protected override IEnumerable<Row> Parse(string content)
        => content.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(line => line.Split(','))
            .Where(p => p.Length == 2)
            .Select(p => new Row(p[0], int.TryParse(p[1], out var n) ? n : 0));
}

public sealed class JsonImporter : FileImporter
{
    protected override IEnumerable<Row> Parse(string content)
        => JsonSerializer.Deserialize<List<Row>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
}

public static class Program
{
    public static int Main()
    {
        var csv = new CsvImporter().Import("sku-1,2\nsku-2,0");
        Console.WriteLine(string.Join(";", csv.Select(r => r.Sku)));
        return 0;
    }
}
