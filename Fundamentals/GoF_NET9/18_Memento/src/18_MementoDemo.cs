namespace GoF.Memento;

public class Editor
{
    private string _text = "";
    public void Type(string t)=> _text += t;
    public string Save()=> _text;
    public void Restore(string m)=> _text = m;
    public string Text => _text;
}
