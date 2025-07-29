using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Application.Dtos;
using Keystore.Domain.Abstractions;
using Keystore.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Keystore.Application.Features.KeyBundle.Queries.GetPrekey;

public sealed class GetSessionKeysQueryHandler(
    IKeyBundleRepository keyBundleRepository,
    ILogger<GetSessionKeysQueryHandler> logger) 
    : IQueryHandler<GetSessionKeysQuery, SessionKeysDto>
{
    public async Task<Result<SessionKeysDto>> Handle(GetSessionKeysQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var keyBundle = await keyBundleRepository.GetByAccessIdAsync(request.AccessId, cancellationToken: cancellationToken);
            if (keyBundle is null)
            {
                return Result.Fail<SessionKeysDto>(Error.NotFound(ErrorCodes.KeyNotFound, "Key not found with access id: {0}", request.AccessId));
            }

            var preKey = keyBundle.ConsumePreKey();
            if (preKey is null)
            {
                return Result.Fail<SessionKeysDto>(Error.NotFound(ErrorCodes.NoPreKeys, "No pre keys found with access id: {0}", request.AccessId));
            }

            var sessionKeysDto = SessionKeysDto.MapToSessionKeysDto(keyBundle, preKey);
            
            return Result.Ok(sessionKeysDto);
            
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Fail<SessionKeysDto>(Error.Failure(ErrorCodes.Server, "Exception occured trying to get key bundle."));
        }
    }
}