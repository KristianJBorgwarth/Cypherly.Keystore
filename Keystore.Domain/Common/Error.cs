﻿namespace Keystore.Domain.Common;

public sealed record Error(string Code, ErrorType Type, string? Description = null)
{
    public static Error NotFound<T>(string id) => new("entity.not.found", ErrorType.NotFound, $"Could not find {typeof(T)} with ID {id}.");
    
    public static Error Unauthorized() => new("unauthorized", ErrorType.Unauthorized);

    public static Error Validation(string description) => new("validation.error", ErrorType.Validation, description);
    
    public static Error Failure() => new("internal.server.error", ErrorType.Failure);
    
    public static Error BadRequest(string code, string description) => new(code, ErrorType.BadRequest, description);
}