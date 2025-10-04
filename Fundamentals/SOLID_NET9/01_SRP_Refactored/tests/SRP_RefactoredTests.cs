using Solid.SRP.Ref;
using Xunit;

public class SRP_Ref_Tests
{
    private sealed class StubMailer : IWelcomeMailer { public void Send(string email) { } }

    [Fact]
    public void Splits_Responsibilities()
    {
        var repo = new InMemoryUserRepository();
        var svc = new UserService(repo, new UserValidator(), new StubMailer());
        svc.Register("a@b");
        Assert.True(repo.Exists("a@b"));
    }
}
