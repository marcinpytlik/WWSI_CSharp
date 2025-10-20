using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class MiscRoutingEndpoints
{
    public static RouteGroupBuilder MapRoutingDemos(this RouteGroupBuilder api)
    {
        var demo = api.MapGroup("/routing");

        // Priority: int vs string
        demo.MapGet("/by-id/{id:int}", (int id) => Results.Ok(new { matched = "int", id }));
        demo.MapGet("/by-id/{id}", (string id) => Results.Ok(new { matched = "string", id }));

        // Query + Headers
        demo.MapGet("/from-header", ([FromHeader(Name = "X-Demo")] string? header, [FromQuery] int take = 5)
            => Results.Ok(new { header = header ?? "(none)", take }));

        // Explicit statuses
        demo.MapPost("/accept", () => Results.Accepted());
        demo.MapDelete("/noop", () => Results.NoContent());

        return api;
    }
}
