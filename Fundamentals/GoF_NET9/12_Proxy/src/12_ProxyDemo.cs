namespace GoF.Proxy;

public interface IRepo { Task<string> Get(int id); }
public class Repo : IRepo { public Task<string> Get(int id)=> Task.FromResult($"#{id}"); }

public class SecureRepoProxy : IRepo
{
    private readonly IRepo _inner; private readonly Func<bool> _can;
    public SecureRepoProxy(IRepo inner, Func<bool> can){ _inner=inner; _can=can; }
    public Task<string> Get(int id)=> _can() ? _inner.Get(id) : throw new UnauthorizedAccessException();
}
