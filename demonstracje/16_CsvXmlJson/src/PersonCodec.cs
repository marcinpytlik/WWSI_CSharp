using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace Demo16;

public sealed record Person(string Name, int Age);

public static class PersonCodec
{
    public static string ToCsv(IEnumerable<Person> people)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Name,Age");
        foreach (var p in people)
            sb.AppendLine($"{Escape(p.Name)},{p.Age}");
        return sb.ToString();
    }

    public static IReadOnlyList<Person> FromCsv(string csv)
    {
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return lines.Skip(1).Select(line =>
        {
            var parts = line.Split(',');
            return new Person(Unescape(parts[0]), int.Parse(parts[1], CultureInfo.InvariantCulture));
        }).ToList();
    }

    public static string ToJson(IEnumerable<Person> people)
        => JsonSerializer.Serialize(people);

    public static IReadOnlyList<Person> FromJson(string json)
        => JsonSerializer.Deserialize<List<Person>>(json) ?? [];

    public static string ToXml(IEnumerable<Person> people)
        => new XElement("people",
            people.Select(p => new XElement("person",
                new XAttribute("name", p.Name),
                new XAttribute("age", p.Age)))).ToString();

    public static IReadOnlyList<Person> FromXml(string xml)
        => XDocument.Parse(xml).Root!.Elements("person")
            .Select(e => new Person((string)e.Attribute("name")!, (int)e.Attribute("age")!))
            .ToList();

    private static string Escape(string value) => value.Replace(",", "\\,");
    private static string Unescape(string value) => value.Replace("\\,", ",");
}

public static class Program
{
    public static int Main()
    {
        Person[] people = [new("Ada", 36), new("Alan", 41)];
        Console.Write(PersonCodec.ToCsv(people));
        Console.WriteLine(PersonCodec.ToJson(people));
        Console.WriteLine(PersonCodec.ToXml(people));
        return 0;
    }
}
