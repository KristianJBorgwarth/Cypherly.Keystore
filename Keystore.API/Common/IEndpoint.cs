namespace Keystore.API.Common;

internal interface IEndpoint
{
    void MapRoutes(IEndpointRouteBuilder routeBuilder);
}