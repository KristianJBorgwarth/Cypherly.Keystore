
namespace Cypherly.Keystore.Domain.Abstractions;

public sealed record Error
{
    public string Code { get; }
    public ErrorType Type { get; }
    public string Description { get; }

    private Error(string code, ErrorType type, string description)
    {
        Code = code;
        Type = type;
        Description = description;
    }

    public static Error NotFound(string code, string description)
    {
        return new Error(code, ErrorType.NotFound, description);
    }

    public static Error Unauthorized(string code, string description)
    {
        return new Error(code, ErrorType.Unauthorized, description);
    }

    public static Error Validation(string description)
    {
        return new Error("Error.Validation", ErrorType.Validation, description);
    }

    public static Error Failure(string code, string description)
    {
        return new Error(code, ErrorType.Failure, description);
    }

    public static Error NoContent(string code, string description)
    {
        return new Error(code, ErrorType.NoContent, description);
    }
}