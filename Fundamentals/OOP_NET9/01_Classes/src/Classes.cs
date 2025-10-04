namespace Oop.Classes;

public class Counter
{
    private int _value;
    public int Value => _value;
    public void Inc() => _value++;
    public void Add(int v) => _value += v;
}
