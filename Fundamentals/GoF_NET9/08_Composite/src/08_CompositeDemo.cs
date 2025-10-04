namespace GoF.Composite;

public interface INode { int Count(); }
public class Leaf : INode { public int Count()=>1; }
public class Group : INode
{
    private readonly List<INode> _children = new();
    public void Add(INode n)=>_children.Add(n);
    public int Count()=>_children.Sum(c=>c.Count());
}
