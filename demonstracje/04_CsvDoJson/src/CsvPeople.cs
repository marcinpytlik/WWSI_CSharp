using System.Text.Json;

namespace Demo04;

public sealed record Person(string Name, int Year);

public static class CsvPeople
{
    public static IReadOnlyList<Person> Parse(string csv)
    {
        var people = new List<Person>();
        using var reader = new StringReader(csv);
        var header = reader.ReadLine();
        if (header is null) return people;

        string? line;
        var number = 1;
        while ((line = reader.ReadLine()) is not null)
        {
            number++;
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split(',');
            if (parts.Length != 2 || !int.TryParse(parts[1], out var year))
                throw new FormatException($"Invalid CSV at line {number}.");
            people.Add(new Person(parts[0].Trim(), year));
        }

        return people;
    }

    public static string ToJson(IEnumerable<Person> people)
        => JsonSerializer.Serialize(people, new JsonSerializerOptions { WriteIndented = true });
}

public static class Program
{
    public static int Main()
    {
        const string csv = "name,year\nAda,1815\nAlan,1912\n";
        Console.WriteLine(CsvPeople.ToJson(CsvPeople.Parse(csv)));
        return 0;
    }
}
