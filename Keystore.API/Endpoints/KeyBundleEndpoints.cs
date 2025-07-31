using Keystore.API.Common;
using Keystore.API.Requests;
using Keystore.Application.Dtos;
using Keystore.Application.Features.KeyBundle.Commands.UploadOTPs;
using Keystore.Application.Features.KeyBundle.Queries.GetPrekey;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace Keystore.API.Endpoints;

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

        group.MapGet("/session", async ([AsParameters] GetSessionKeysRequest req, ISender sender) =>
            {
                var query = new GetSessionKeysQuery { AccessKey = req.AccessKey };
                var result = await sender.Send(query);
                return result.Success ? Results.Ok() : result.ToProblemDetails();
            })
            .Produces<SessionKeysDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        
        group.MapPost("/otps", async ([FromBody] UploadOneTimePreKeysRequest req, ISender sender, HttpContext ctx) =>
        {
            var userId = ctx.User.GetUserId();
            var cmd = new UploadOneTimePreKeysCommand { Id = userId, PreKeys = req.PreKeys };
            var result = await sender.Send(cmd);
            return result.Success ? Results.Ok() : result.ToProblemDetails();
        })
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}