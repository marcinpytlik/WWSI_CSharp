namespace GoF.Mediator;

public interface IMediator{ void Publish(string topic, object payload); void Subscribe(string t, Action<object> h); }

public class SimpleMediator : IMediator
{
    private readonly Dictionary<string,List<Action<object>>> _subs = new();
    public void Subscribe(string t, Action<object> h){ if(!_subs.ContainsKey(t)) _subs[t]=new(); _subs[t].Add(h); }
    public void Publish(string t, object p){ if(_subs.TryGetValue(t, out var hs)) foreach(var h in hs) h(p); }
}
