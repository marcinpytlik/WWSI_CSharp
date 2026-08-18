using Solid.SRP.Anti;
using Xunit;

public class SRP_Anti_Tests
{
    [Fact]
    public void Does_Everything_In_One_Class()
    {
        var svc = new UserService();
        svc.Register("a@b");
        Assert.True(svc.Exists("a@b"));
        Assert.Contains("WelcomeMail:a@b", svc.Log);
    }
}
