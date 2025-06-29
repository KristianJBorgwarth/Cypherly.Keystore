using Cypherly.Keystore.API.Extensions;
using Cypherly.Keystore.API.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cypherly.Keystore.API.Endpoints;

public sealed class KeyBundleEndpoints : IEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("api/keys")
            .WithTags("Key Bundles")
            .RequireAuthorization();

        group.MapPost("/", async ([FromBody] CreateKeyBundleRequest req, ISender sender, HttpContext ctx) =>
            {
                var userId = ctx.User.GetUserId();
                var cmd = req.MapToCommand(userId);
                var result = await sender.Send(cmd);

                return result.Success ? Results.Ok() : result.ToProblemDetails();
            })
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}