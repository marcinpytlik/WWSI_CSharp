namespace GoF.Chain;

public abstract class Handler
{
    protected Handler? Next;
    public Handler SetNext(Handler next){ Next = next; return next; }
    public virtual string Handle(string req)=> Next?.Handle(req) ?? req;
}
public class TrimHandler:Handler{ public override string Handle(string r)=> base.Handle(r.Trim()); }
public class LowerHandler:Handler{ public override string Handle(string r)=> base.Handle(r.ToLowerInvariant()); }
