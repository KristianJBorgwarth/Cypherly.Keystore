using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Keystore.Infrastructure.Persistence.Context;
using Keystore.Test.Integration.Setup;
using Microsoft.AspNetCore.Mvc;

namespace Keystore.Test.Integration.EndpointTest;

public class UnauthorizedExceptionHandlerTest(IntegrationTestFactory<Program, KeystoreDbContext> factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task CreateKeyBundle_Given_TokenWithoutRequiredClaims_Should_Return401()
    {
        // Arrange
        // TestAuthHandler issues no user_id/device_id/connection_id claims
        var body = new
        {
            AccessKey = Guid.NewGuid(),
            IdentityKey = new byte[] { 1 },
            RegistrationId = 1,
            SignedPrekeyId = 1,
            SignedPreKeyPublic = new byte[] { 1 },
            SignedPreKeySignature = new byte[] { 1 },
            PreKeys = Array.Empty<object>(),
            SignedPreKeyTimestamp = DateTimeOffset.UtcNow,
            KyberPreKeyId = 1,
            KyberPreKeyPublic = new byte[] { 1 },
            KyberPreKeySignature = new byte[] { 1 }
        };

        // Act
        var response = await Client.PostAsJsonAsync("api/keys", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Title.Should().Be("Unauthorized");
    }
}
