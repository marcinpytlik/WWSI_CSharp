namespace Demo12;

/// <summary>
/// Dwa konta SQL — zasada minimalnych uprawnień.
/// <c>demo12_deploy</c> wdraża schemat (DDL / migracje EF).
/// <c>demo12_app</c> tylko czyta i zapisuje dane (DML).
/// Hasła wyłącznie do lokalnego Dockera na sali.
/// </summary>
public static class SqlAccounts
{
    public const string Server = "localhost,1433";
    public const string DeployUser = "demo12_deploy";
    public const string AppUser = "demo12_app";
    public const string DeployPassword = "Demo12_Deploy_Pass!";
    public const string AppPassword = "Demo12_App_Pass!";
    public const string CodeFirstDatabase = "Demo12_CodeFirst";
    public const string DatabaseFirstDatabase = "Demo12_DbFirst";

    public static string CodeFirstDeploy => For(CodeFirstDatabase, DeployUser, DeployPassword);
    public static string CodeFirstApp => For(CodeFirstDatabase, AppUser, AppPassword);
    public static string DatabaseFirstDeploy => For(DatabaseFirstDatabase, DeployUser, DeployPassword);
    public static string DatabaseFirstApp => For(DatabaseFirstDatabase, AppUser, AppPassword);

    public static string For(string database, string user, string password)
        => $"Server={Server};Database={database};User Id={user};Password={password};TrustServerCertificate=True;";

    public static bool IsAppConnection(string connectionString)
        => connectionString.Contains($"User Id={AppUser}", StringComparison.OrdinalIgnoreCase)
           && !connectionString.Contains($"User Id={DeployUser}", StringComparison.OrdinalIgnoreCase);

    public static bool IsDeployConnection(string connectionString)
        => connectionString.Contains($"User Id={DeployUser}", StringComparison.OrdinalIgnoreCase)
           && !connectionString.Contains($"User Id={AppUser}", StringComparison.OrdinalIgnoreCase);
}
