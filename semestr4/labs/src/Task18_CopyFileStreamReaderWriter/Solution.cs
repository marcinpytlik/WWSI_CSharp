namespace Task18_CopyFileStreamReaderWriter;

public static class FileCopier
{
    public static void CopyTextFile(string src, string dst)
    {
        using var reader = new StreamReader(src);
        using var writer = new StreamWriter(dst, append: false);

        string? line;
        while ((line = reader.ReadLine()) is not null)
            writer.WriteLine(line);
    }
}
