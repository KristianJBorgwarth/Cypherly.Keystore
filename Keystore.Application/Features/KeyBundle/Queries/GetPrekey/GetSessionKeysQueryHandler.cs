using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Application.Dtos;
using Keystore.Domain.Abstractions;
using Keystore.Domain.Common;
using Keystore.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Keystore.Application.Features.KeyBundle.Queries.GetPrekey;

public sealed class GetSessionKeysQueryHandler(
    IKeyBundleRepository keyBundleRepository,
    IUnitOfWork unitOfWork, 
    ILogger<GetSessionKeysQueryHandler> logger)
    : IQueryHandler<GetSessionKeysQuery, SessionKeysDto>
{
    public async Task<Result<SessionKeysDto>> Handle(GetSessionKeysQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var keyBundle = await keyBundleRepository.GetByAccessIdAsync(query.AccessKey, cancellationToken: cancellationToken);
            if (keyBundle is null)
            {
                return Result.Fail<SessionKeysDto>(Error.NotFound("Key not found"));
            }

            var preKey = keyBundle.ConsumePreKey();

            var sessionKeysDto = SessionKeysDto.MapToSessionKeysDto(keyBundle, preKey);
            
            keyBundle.AddDomainEvent(new KeyCountEvent {TenantId = keyBundle.Id, ConnectionId = keyBundle.AccessKey});

            await unitOfWork.SaveChangesAsync(CancellationToken.None);
            
            return Result.Ok(sessionKeysDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while retrieving the key bundle.");
            return Result.Fail<SessionKeysDto>(Error.Failure("Exception occured trying to get key bundle."));
        }
    }
}