using Oop.Inheritance;
using Xunit;

public class InheritanceTests
{
    [Fact] public void Dog_Speaks_Woof() => Assert.Equal("Woof", new Dog().Speak());
    [Fact] public void Cat_Speaks_Meow() => Assert.Equal("Meow", new Cat().Speak());
}
