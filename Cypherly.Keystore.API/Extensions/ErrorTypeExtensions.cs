using Cypherly.Keystore.Domain.Abstractions;

namespace Cypherly.Keystore.API.Extensions;

public static class ErrorTypeExtensions
{
    public static int ToHttpStatusCode(this ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Failure => StatusCodes.Status400BadRequest,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    public static string ToProblemDetailsTitle(this ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Failure => "Bad Request",
            ErrorType.Validation => "Bad Request",
            ErrorType.NotFound => "Not Found",
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.NoContent => "No Content",
            _ => "Internal Server Error"
        };
    }

    public static string ToProblemDetailsTypeUri(this ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1", // 400
            ErrorType.Failure => "https://tools.ietf.org/html/rfc7231#section-6.5.1",    // 400
            ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",   // 404
            ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1", // 401
            ErrorType.NoContent => "https://tools.ietf.org/html/rfc7231#section-6.5.7", // 204
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1", // 500
        };
    }
}