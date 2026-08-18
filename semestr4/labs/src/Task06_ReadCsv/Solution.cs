namespace Task06_ReadCsv;

public static class CsvReader
{
    public static IEnumerable<string[]> ReadCsv(string path, bool skipHeader = false, char separator = ',')
    {
        var lines = File.ReadLines(path);
        if (skipHeader) lines = lines.Skip(1);

        foreach (var line in lines)
            yield return line.Split(separator);
    }
}
