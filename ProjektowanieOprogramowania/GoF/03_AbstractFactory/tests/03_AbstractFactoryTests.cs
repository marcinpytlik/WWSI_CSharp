using GoF.AbstractFactory;
using Xunit;

public class AbstractFactoryTests
{
    [Fact] public void DarkFactory_Creates_Dark_Controls()
    {
        IUiFactory f = new DarkUiFactory();
        Assert.Equal("DarkButton", f.Button().Kind());
        Assert.Equal("DarkTextBox", f.TextBox().Kind());
    }
}
