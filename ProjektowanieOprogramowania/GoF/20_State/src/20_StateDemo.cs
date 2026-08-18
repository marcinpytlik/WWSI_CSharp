namespace GoF.State;

public interface IState { IState Next(); string Name { get; } }
public class Draft: IState { public IState Next()=> new Published(); public string Name=>"Draft"; }
public class Published: IState { public IState Next()=> new Archived(); public string Name=>"Published"; }
public class Archived: IState { public IState Next()=> this; public string Name=>"Archived"; }

public class Doc
{
    public IState State { get; private set; } = new Draft();
    public void Advance()=> State = State.Next();
}
