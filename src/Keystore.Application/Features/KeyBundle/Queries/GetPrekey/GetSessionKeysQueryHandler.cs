using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Application.Dtos;
using Keystore.Application.Exceptions;
using Keystore.Domain.Common;
using Keystore.Domain.Events;

namespace Keystore.Application.Features.KeyBundle.Queries.GetPrekey;

public sealed class GetSessionKeysQueryHandler(
    IKeyBundleRepository keyBundleRepository,
    IUnitOfWork unitOfWork)
    : IQueryHandler<GetSessionKeysQuery, SessionKeysDto>
{
    public async ValueTask<Result<SessionKeysDto>> Handle(GetSessionKeysQuery query, CancellationToken cancellationToken)
    {
        var keyBundle = await keyBundleRepository.GetAsync(new KeyBundleByAccessKeyWithPreKeysSpec(query.AccessKey), cancellationToken);
        if (keyBundle is null)
        {
            return Result.Fail<SessionKeysDto>(Error.NotFound<Domain.Aggregates.KeyBundle>(query.AccessKey.ToString()));
        }

        var preKey = keyBundle.ConsumePreKey();

        var sessionKeysDto = SessionKeysDto.MapToSessionKeysDto(keyBundle, preKey);

        keyBundle.AddDomainEvent(new KeyCountEvent { TenantId = keyBundle.Id, ConnectionId = keyBundle.AccessKey });

        try
        {
            await unitOfWork.SaveChangesAsync(CancellationToken.None);
        }
        catch (ConcurrencyConflictException e)
        {
            return Result.Fail<SessionKeysDto>(Error.Conflict(e.Message));
        }

        return Result.Ok(sessionKeysDto);
    }
}
