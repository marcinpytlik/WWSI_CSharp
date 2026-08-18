using GoF.Proxy;
using Xunit;

public class ProxyTests
{
    [Fact]
    public async Task Blocks_When_Not_Authorized()
    {
        var p = new SecureRepoProxy(new Repo(), () => false);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => p.Get(1));
    }
}
