namespace Keystore.Domain.Common;

public sealed record Error
{
    public string Code { get; }
    public ErrorType Type { get; }
    public string? Description { get; }

    private Error(string code, ErrorType type, string? description = null)
    {
        Code = code;
        Type = type;
        Description = description;
    }

    public static Error NotFound(string code, string? description = null )
    {
        return new Error(code, ErrorType.NotFound, description);
    }
    
    public static Error Unauthorized(string code, string description)
    {
        return new Error(code, ErrorType.Unauthorized, description);
    }

    public static Error Validation(string description)
    {
        return new Error("validation.error", ErrorType.Validation, description);
    }

    public static Error Failure(string description)
    {
        return new Error("internal.server.error", ErrorType.Failure, description);
    }
    
    public static Error BadRequest(string code, string description)
    {
        return new Error(code, ErrorType.BadRequest, description);
    }
}