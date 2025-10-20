namespace Api.Endpoints;

public static class HealthEndpoints
{
    public static RouteGroupBuilder MapHealth(this RouteGroupBuilder group)
    {
        group.MapGet("/health", () => Results.Ok(new { status = "ok" }))
             .WithName("Health")
             .Produces(StatusCodes.Status200OK);
        return group;
    }
}
