using Cypherly.Keystore.Application.Abstractions;
using Cypherly.Keystore.Application.Contracts;
using Cypherly.Keystore.Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace Cypherly.Keystore.Application.Features.KeyBundle.Command.Create;

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
            var keyBundle = new Domain.Aggregates.KeyBundle(
                request.UserId,
                request.AccessKey,
                request.IdentityKey,
                request.RegistrationId,
                request.SignedPrekeyId,
                request.SignedPreKeyPublic,
                request.SignedPreKeySignature,
                request.SignedPreKeyTimestamp);

            await keyBundleRepository.CreateAsync(keyBundle, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogCritical("An error occurred while creating a key bundle: {Message}", ex.Message);
            return Result.Fail(Error.Failure("KEY_BUNDLE_CREATION_FAILED", "Failed to create key bundle"));
        }
    }
}