namespace Demo41.Gateway;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var testing = builder.Configuration.GetValue("Testing", false);

        if (!testing)
            builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        var app = builder.Build();
        app.MapGet("/health", () => Results.Ok(new { status = "ok", role = "gateway" }));
        if (!testing)
            app.MapReverseProxy();
        app.Run();
    }
}
