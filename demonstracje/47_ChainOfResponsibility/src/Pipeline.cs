using System.Text.RegularExpressions;

namespace Demo47;

public abstract class CodeHandler
{
    private CodeHandler? _next;

    public CodeHandler Then(CodeHandler next)
    {
        _next = next;
        return next;
    }

    public string? Handle(string input)
    {
        var error = Check(input);
        return error ?? _next?.Handle(input);
    }

    protected abstract string? Check(string input);
}

public sealed class EmptyHandler : CodeHandler
{
    protected override string? Check(string input)
        => string.IsNullOrWhiteSpace(input) ? "Code is required." : null;
}

public sealed class FormatHandler : CodeHandler
{
    private static readonly Regex Pattern = new(@"^[A-Z]{3}-\d{3}$", RegexOptions.Compiled);

    protected override string? Check(string input)
        => Pattern.IsMatch(input) ? null : "Code must match AAA-000.";
}

public sealed class LimitHandler : CodeHandler
{
    protected override string? Check(string input)
    {
        var n = int.Parse(input.Split('-')[1]);
        return n > 500 ? "Numeric part must be <= 500." : null;
    }
}

public static class CodePipeline
{
    public static CodeHandler Create()
    {
        var empty = new EmptyHandler();
        empty.Then(new FormatHandler()).Then(new LimitHandler());
        return empty;
    }
}
