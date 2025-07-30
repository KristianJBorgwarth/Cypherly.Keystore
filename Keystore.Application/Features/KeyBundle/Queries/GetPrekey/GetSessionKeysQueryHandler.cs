using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Application.Dtos;
using Keystore.Domain.Abstractions;
using Keystore.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Keystore.Application.Features.KeyBundle.Queries.GetPrekey;

public sealed class GetSessionKeysQueryHandler(
    IKeyBundleRepository keyBundleRepository,
    IUnitOfWork unitOfWork, 
    ILogger<GetSessionKeysQueryHandler> logger)
    : IQueryHandler<GetSessionKeysQuery, SessionKeysDto>
{
    public async Task<Result<SessionKeysDto>> Handle(GetSessionKeysQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var keyBundle = await keyBundleRepository.GetByAccessIdAsync(request.AccessKey, cancellationToken: cancellationToken);
            if (keyBundle is null)
            {
                return Result.Fail<SessionKeysDto>(Error.NotFound(ErrorCodes.KeyNotFound, "Key not found with access id: {0}", request.AccessKey));
            }

            var preKey = keyBundle.ConsumePreKey();

            var sessionKeysDto = SessionKeysDto.MapToSessionKeysDto(keyBundle, preKey);

            await unitOfWork.SaveChangesAsync(CancellationToken.None);
            return Result.Ok(sessionKeysDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Fail<SessionKeysDto>(Error.Failure(ErrorCodes.Server, "Exception occured trying to get key bundle."));
        }
    }
}