﻿using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Keystore.Application.Features.KeyBundle.Queries.GetPreKeyCount;

public sealed class GetPreKeyCountQueryHandler(
    IKeyBundleRepository keyBundleRepository,
    ILogger<GetPreKeyCountQueryHandler> logger) 
    : IQueryHandler<GetPreKeyCountQuery, GetPreKeyCountDto>
{
    public async Task<Result<GetPreKeyCountDto>> Handle(GetPreKeyCountQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var keyBundle = await keyBundleRepository.GetByIdWithPreKeysAsync(query.TenantId, cancellationToken);
            if (keyBundle is null)
            {
                return Result.Fail<GetPreKeyCountDto>(Error.NotFound("Key not found"));
            }
            
            var count = keyBundle.PreKeys.Count;

            return new GetPreKeyCountDto { KeyCount = count };
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An exception occured while getting pre key count for user with ID: {ID}", query.TenantId);
            return Result.Fail<GetPreKeyCountDto>(Error.Failure("An exception occured while retrieving prekey count") );
        }
    }
}