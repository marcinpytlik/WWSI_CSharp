using System.Xml.Linq;

namespace Demo46;

public sealed record Quote(string Id, string Text, string Author);

public interface IQuoteClient
{
    Task<Quote?> GetAsync(string id, CancellationToken cancellationToken);
}

public interface IXmlQuoteSource
{
    Task<string?> GetXmlAsync(string id, CancellationToken cancellationToken);
}

public sealed class XmlQuoteAdapter : IQuoteClient
{
    private readonly IXmlQuoteSource _source;
    public XmlQuoteAdapter(IXmlQuoteSource source) => _source = source;

    public async Task<Quote?> GetAsync(string id, CancellationToken cancellationToken)
    {
        var xml = await _source.GetXmlAsync(id, cancellationToken);
        if (xml is null) return null;
        var doc = XDocument.Parse(xml);
        var root = doc.Root ?? throw new InvalidOperationException("Empty XML.");
        return new Quote(
            (string?)root.Element("id") ?? id,
            (string?)root.Element("text") ?? "",
            (string?)root.Element("author") ?? "");
    }
}

public sealed class InMemoryXmlQuoteSource : IXmlQuoteSource
{
    private readonly Dictionary<string, string> _xml = new(StringComparer.OrdinalIgnoreCase)
    {
        ["q1"] = """
                 <quote>
                   <id>q1</id>
                   <text>Talk is cheap. Show me the code.</text>
                   <author>Linus Torvalds</author>
                 </quote>
                 """
    };

    public Task<string?> GetXmlAsync(string id, CancellationToken cancellationToken)
        => Task.FromResult(_xml.TryGetValue(id, out var xml) ? xml : null);
}
