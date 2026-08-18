using Oop.Generics;
using Xunit;

public class GenericsTests
{
    [Fact]
    public void Stores_Type_Safely()
    {
        var repo = new InMemoryRepository<string>();
        repo.Add(1, "A");
        Assert.Equal("A", repo.Get(1));
    }
}
