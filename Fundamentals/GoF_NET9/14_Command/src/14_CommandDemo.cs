namespace GoF.Command;

public interface ICommand { void Execute(); void Undo(); }

public class AddItem : ICommand
{
    private readonly List<int> _list; private readonly int _v;
    public AddItem(List<int> l,int v){ _list=l; _v=v; }
    public void Execute()=>_list.Add(_v);
    public void Undo()=>_list.Remove(_v);
}
