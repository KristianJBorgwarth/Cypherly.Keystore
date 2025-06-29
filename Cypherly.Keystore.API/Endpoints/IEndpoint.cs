namespace Cypherly.Keystore.API.Endpoints;

internal interface IEndpoint
{
    void MapRoutes(IEndpointRouteBuilder routeBuilder);
}