namespace GoF.Observer;

public class Topic
{
    public event Action<string>? OnMsg;
    public void Publish(string m)=> OnMsg?.Invoke(m);
}
