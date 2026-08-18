namespace GoF.Visitor;

public interface INode { void Accept(IVisitor v); }
public class Number : INode { public int V; public Number(int v)=>V=v; public void Accept(IVisitor v)=> v.Visit(this); }

public interface IVisitor { void Visit(Number n); }
public class SumVisitor : IVisitor
{
    public int Sum { get; private set; }
    public void Visit(Number n) => Sum += n.V;
}
