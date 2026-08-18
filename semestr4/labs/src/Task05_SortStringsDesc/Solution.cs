namespace Task05_SortStringsDesc;

public static class Sorter
{
    public static List<string> SortDescending(IEnumerable<string> items)
        => items.OrderByDescending(x => x, StringComparer.Ordinal).ToList();
}
