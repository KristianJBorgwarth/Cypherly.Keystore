using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Domain.Common;

namespace Keystore.Application.Features.KeyBundle.Queries.GetPreKeyCount;

public sealed class GetPreKeyCountQueryHandler(IKeyBundleRepository keyBundleRepository) : IQueryHandler<GetPreKeyCountQuery, GetPreKeyCountDto>
{
    public async Task<Result<GetPreKeyCountDto>> Handle(GetPreKeyCountQuery query, CancellationToken cancellationToken)
    {
        var keyBundle = await keyBundleRepository.GetByIdWithPreKeysAsync(query.Id, cancellationToken);
        if (keyBundle is null)
        {
            return Result.Fail<GetPreKeyCountDto>(Error.NotFound<Domain.Aggregates.KeyBundle>(query.Id.ToString()));
        }

        var count = keyBundle.PreKeys.Count;

        return new GetPreKeyCountDto { KeyCount = count };
    }
}
