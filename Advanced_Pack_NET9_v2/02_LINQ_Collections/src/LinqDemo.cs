
namespace Advanced.LinqCollections;

public static class LinqDemo
{
    public static IEnumerable<string> SelectActiveUpper(IEnumerable<(string Name, bool Active)> src)
        => src.Where(x => x.Active).Select(x => x.Name.ToUpperInvariant());

    public static int SumSpan(ReadOnlySpan<int> span)
    {
        var sum = 0;
        foreach (var x in span) sum += x;
        return sum;
    }
}
