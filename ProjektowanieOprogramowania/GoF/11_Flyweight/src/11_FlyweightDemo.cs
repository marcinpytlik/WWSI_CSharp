namespace GoF.Flyweight;

public record Glyph(char Char);
public class GlyphFactory
{
    private readonly Dictionary<char,Glyph> _cache = new();
    public Glyph Get(char c) => _cache.TryGetValue(c, out var g) ? g : _cache[c] = new Glyph(c);
}
