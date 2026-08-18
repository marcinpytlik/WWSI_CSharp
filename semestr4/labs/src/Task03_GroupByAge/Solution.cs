namespace Task03_GroupByAge;

public sealed record Person(string Name, int Age);

public static class Grouper
{
    public static IReadOnlyDictionary<int, List<Person>> GroupByAge(IEnumerable<Person> people)
        => people.GroupBy(p => p.Age)
                 .ToDictionary(g => g.Key, g => g.ToList());
}
