namespace GoF.Builder;

public class QueryBuilder
{
    private string _select = "*";
    private string _from = "";
    private string? _where;

    public QueryBuilder From(string t) { _from = t; return this; }
    public QueryBuilder Where(string w) { _where = w; return this; }
    public string Build() => $"SELECT {_select} FROM {_from}" + (_where is null ? "" : $" WHERE {_where}");
}
