
using System.Text.RegularExpressions;

namespace Orders.Rich.Domain.Primitives;
public sealed record Email
{
    public string Value { get; }
    private Email(string value) => Value = value;
    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, ".+@.+"))
            throw new ArgumentException("Invalid email");
        return new Email(value);
    }
    public override string ToString() => Value;
}
