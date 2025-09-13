using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Domain.Abstractions;
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
            return Result.Fail(Error.Failure());
        }
    }
}