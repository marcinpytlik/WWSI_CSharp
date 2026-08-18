using System.Text.Json;
using Demo12;
using Xunit;

namespace Demo12.Tests;

public class LeastPrivilegeTests
{
    [Fact]
    public void AppConnection_UsesAppLogin_NotDeploy()
    {
        Assert.True(SqlAccounts.IsAppConnection(SqlAccounts.CodeFirstApp));
        Assert.True(SqlAccounts.IsAppConnection(SqlAccounts.DatabaseFirstApp));
        Assert.False(SqlAccounts.IsDeployConnection(SqlAccounts.CodeFirstApp));
        Assert.DoesNotContain(SqlAccounts.DeployUser, SqlAccounts.CodeFirstApp, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(SqlAccounts.DeployUser, SqlAccounts.DatabaseFirstApp, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeployConnection_UsesDeployLogin_NotApp()
    {
        Assert.True(SqlAccounts.IsDeployConnection(SqlAccounts.CodeFirstDeploy));
        Assert.True(SqlAccounts.IsDeployConnection(SqlAccounts.DatabaseFirstDeploy));
        Assert.False(SqlAccounts.IsAppConnection(SqlAccounts.CodeFirstDeploy));
        Assert.DoesNotContain($"User Id={SqlAccounts.AppUser}", SqlAccounts.CodeFirstDeploy, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Appsettings_KeepTwoAccounts()
    {
        AssertTwoAccounts("codefirst-appsettings.json", SqlAccounts.CodeFirstDatabase);
        AssertTwoAccounts("dbfirst-appsettings.json", SqlAccounts.DatabaseFirstDatabase);
    }

    private static void AssertTwoAccounts(string fileName, string database)
    {
        var path = Path.Combine(AppContext.BaseDirectory, fileName);
        using var doc = JsonDocument.Parse(File.ReadAllText(path));
        var cs = doc.RootElement.GetProperty("ConnectionStrings");
        var deploy = cs.GetProperty("Deploy").GetString()!;
        var app = cs.GetProperty("App").GetString()!;
        Assert.Contains(database, deploy, StringComparison.Ordinal);
        Assert.Contains(database, app, StringComparison.Ordinal);
        Assert.True(SqlAccounts.IsDeployConnection(deploy));
        Assert.True(SqlAccounts.IsAppConnection(app));
    }
}
