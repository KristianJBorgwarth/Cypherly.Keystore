using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Application.Dtos;
using Keystore.Domain.Common;
using Keystore.Domain.Events;

namespace Keystore.Application.Features.KeyBundle.Queries.GetPrekey;

public sealed class GetSessionKeysQueryHandler(
    IKeyBundleRepository keyBundleRepository,
    IUnitOfWork unitOfWork)
    : IQueryHandler<GetSessionKeysQuery, SessionKeysDto>
{
    public async Task<Result<SessionKeysDto>> Handle(GetSessionKeysQuery query, CancellationToken cancellationToken)
    {
        var keyBundle = await keyBundleRepository.GetByAccessIdAsync(query.AccessKey, cancellationToken: cancellationToken);
        if (keyBundle is null)
        {
            return Result.Fail<SessionKeysDto>(Error.NotFound<Domain.Aggregates.KeyBundle>(query.AccessKey.ToString()));
        }

        var preKey = keyBundle.ConsumePreKey();

        var sessionKeysDto = SessionKeysDto.MapToSessionKeysDto(keyBundle, preKey);

        keyBundle.AddDomainEvent(new KeyCountEvent { TenantId = keyBundle.Id, ConnectionId = keyBundle.AccessKey });

        await unitOfWork.SaveChangesAsync(CancellationToken.None);

        return Result.Ok(sessionKeysDto);
    }
}
