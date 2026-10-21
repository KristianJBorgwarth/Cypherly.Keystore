using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Domain.Common;
using Keystore.Domain.Entities;

namespace Keystore.Application.Features.KeyBundle.Commands.UploadOTPs;

public sealed class UploadOneTimePreKeysCommandHandler(
    IKeyBundleRepository keyBundleRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UploadOneTimePreKeysCommand>
{
    public async ValueTask<Result> Handle(UploadOneTimePreKeysCommand request, CancellationToken cancellationToken)
    {
        var keyBundle = await keyBundleRepository.GetAsync(new KeyBundleWithPreKeysSpec(request.Id), cancellationToken);
        if (keyBundle is null)
        {
            return Result.Fail(Error.NotFound<Domain.Aggregates.KeyBundle>(request.Id.ToString()));
        }

        var preKeys = request.PreKeys.Select(x =>
            new PreKey(
                id: Guid.NewGuid(),
                keyBundleId: keyBundle.Id,
                keyId: x.KeyId,
                publicKey: x.PublicKey))
            .ToList();

        var uploaded = keyBundle.UploadPreKeys(preKeys);
        if (uploaded.Success is false)
            return uploaded;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();

    }
}
