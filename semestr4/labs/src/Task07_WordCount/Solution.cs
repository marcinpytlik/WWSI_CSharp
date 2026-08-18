namespace Task07_WordCount;

public static class WordCounter
{
    public static IReadOnlyDictionary<string, int> CountWordsFromFile(string path)
    {
        var text = File.ReadAllText(path);

        var words = text
            .ToLowerInvariant()
            .Split(new[] { ' ', '\r', '\n', '\t', '.', ',', ';', ':', '!', '?', '"', '(', ')', '[', ']', '{', '}', '-', '_' },
                   StringSplitOptions.RemoveEmptyEntries);

        return words.GroupBy(w => w)
                    .ToDictionary(g => g.Key, g => g.Count());
    }
}
