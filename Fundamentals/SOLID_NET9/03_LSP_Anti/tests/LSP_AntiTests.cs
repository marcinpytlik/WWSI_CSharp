using Solid.LSP.Anti;
using Xunit;

public class LSP_Anti_Tests
{
    [Fact]
    public void Square_Breaks_Rectangle_Expectation()
    {
        Rectangle r = new Square();
        r.Width = 2;   // klient oczekuje niezależności wymiarów
        r.Height = 3;  // dla Square wysokość ustawia też szerokość
        Assert.NotEqual(6, r.Area()); // nie 2*3 → naruszenie oczekiwań
    }
}
