using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Keystore.Application.Features.KeyBundle.Commands.Create;

public sealed class CreateKeyBundleCommandHandler(
    IKeyBundleRepository keyBundleRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateKeyBundleCommandHandler> logger)
    : ICommandHandler<CreateKeyBundleCommand>
{
    public async Task<Result> Handle(CreateKeyBundleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var keyBundle = MapToBundle(request);
            await keyBundleRepository.CreateAsync(keyBundle, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogCritical("An error occurred while creating a key bundle: {Message}", ex.Message);
            return Result.Fail(Error.Failure());
        }
    }

    private static Domain.Aggregates.KeyBundle MapToBundle(CreateKeyBundleCommand request)
    {
        var keyBundle = new Domain.Aggregates.KeyBundle(
            id: request.DeviceId,
            userId: request.TenantId,
            accessKey: request.AccessKey,
            identityKey: request.IdentityKey,
            registrationId: request.RegistrationId,
            signedPrekeyId: request.SignedPrekeyId,
            signedPreKeyPublic: request.SignedPreKeyPublic,
            signedPreKeySignature: request.SignedPreKeySignature,
            signedPreKeyTimestamp: request.SignedPreKeyTimestamp);

        keyBundle.UploadPreKeys([.. request.PreKeys.Select(x => new Domain.Entities.PreKey(
            id: Guid.NewGuid(), 
            keyBundleId:
            request.DeviceId, 
            keyId: x.KeyId, 
            publicKey: x.PublicKey))]);

        return keyBundle;
    } 
}
