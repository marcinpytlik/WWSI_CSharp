namespace GoF.AbstractFactory;

public interface IButton { string Kind(); }
public interface ITextBox { string Kind(); }

public class DarkButton : IButton { public string Kind() => "DarkButton"; }
public class DarkTextBox : ITextBox { public string Kind() => "DarkTextBox"; }

public class LightButton : IButton { public string Kind() => "LightButton"; }
public class LightTextBox : ITextBox { public string Kind() => "LightTextBox"; }

public interface IUiFactory { IButton Button(); ITextBox TextBox(); }

public class DarkUiFactory : IUiFactory
{
    public IButton Button() => new DarkButton();
    public ITextBox TextBox() => new DarkTextBox();
}
public class LightUiFactory : IUiFactory
{
    public IButton Button() => new LightButton();
    public ITextBox TextBox() => new LightTextBox();
}
