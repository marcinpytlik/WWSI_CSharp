namespace Oop.Events;

public delegate void Alert(string message);

public class Alarm
{
    public event Alert? OnAlert;
    public void Trigger(string m) => OnAlert?.Invoke(m);
}
