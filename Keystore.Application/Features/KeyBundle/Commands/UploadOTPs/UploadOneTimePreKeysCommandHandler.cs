using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Domain.Common;
using Keystore.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Keystore.Application.Features.KeyBundle.Commands.UploadOTPs;

public sealed class UploadOneTimePreKeysCommandHandler(
    IKeyBundleRepository keyBundleRepository,
    IUnitOfWork unitOfWork,
    ILogger<UploadOneTimePreKeysCommandHandler> logger)
    : ICommandHandler<UploadOneTimePreKeysCommand>
{
    public async Task<Result> Handle(UploadOneTimePreKeysCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var keyBundle = await keyBundleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (keyBundle is null)
            {
                return Result.Fail(Error.NotFound(ErrorCodes.KeyNotFound, "Key not found with access id: {0}", request.Id));
            }
            
            var preKeys = request.PreKeys.Select(x=> 
                new PreKey(
                    id: Guid.NewGuid(),
                    keyBundleId: keyBundle.Id, 
                    keyId: x.KeyId, 
                    publicKey: x.PublicKey))
                .ToList();
            
            keyBundle.UploadPreKeys(preKeys);
            
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Error uploading key bundle");
            return Result.Fail(Error.Failure(ErrorCodes.Server, "Error uploading OTPS"));
        }
    }
}