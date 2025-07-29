using Keystore.Application.Features.KeyBundle.Queries.GetPrekey;
using Keystore.Infrastructure.Persistence.Context;
using Keystore.Test.Integration.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Keystore.Test.Integration.QueryTest;

public class SessionKeysQueryHandlerTest : IntegrationTestBase
{
    private readonly GetSessionKeysQueryHandler _sut;
    
    public SessionKeysQueryHandlerTest(IntegrationTestFactory<Program, KeystoreDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _sut = scope.ServiceProvider.GetRequiredService<GetSessionKeysQueryHandler>();
    }

    [Fact]
    public async Task Handle_GivenValidRequest_ShouldReturn_SessionKeysDto()
    {
        // Arrange
    }
}