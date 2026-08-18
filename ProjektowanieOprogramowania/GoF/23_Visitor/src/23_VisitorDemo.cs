namespace GoF.Visitor;

public interface INode
{
    void Accept(IVisitor visitor);
}

public sealed class Number : INode
{
    public int Value { get; }
    public Number(int value) => Value = value;
    public void Accept(IVisitor visitor) => visitor.Visit(this);
}

public sealed class AddNode : INode
{
    public INode Left { get; }
    public INode Right { get; }

    public AddNode(INode left, INode right)
    {
        Left = left;
        Right = right;
    }

    public void Accept(IVisitor visitor) => visitor.Visit(this);
}

public interface IVisitor
{
    void Visit(Number number);
    void Visit(AddNode add);
}

public sealed class SumVisitor : IVisitor
{
    public int Sum { get; private set; }

    public void Visit(Number number) => Sum += number.Value;

    public void Visit(AddNode add)
    {
        add.Left.Accept(this);
        add.Right.Accept(this);
    }
}

public sealed class PrintVisitor : IVisitor
{
    private readonly List<string> _tokens = new();
    public string Text => string.Join(" ", _tokens);

    public void Visit(Number number) => _tokens.Add(number.Value.ToString());

    public void Visit(AddNode add)
    {
        _tokens.Add("(");
        add.Left.Accept(this);
        _tokens.Add("+");
        add.Right.Accept(this);
        _tokens.Add(")");
    }
}
