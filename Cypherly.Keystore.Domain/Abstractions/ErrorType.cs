namespace Cypherly.Keystore.Domain.Abstractions;

public enum ErrorType
{
    Failure = 0,
    Validation = 1, // 400 Bad Request
    NotFound = 2, // 404 Not Found
    Unauthorized = 3, // 401 Unauthorized
    NoContent = 4, // 204 No Content
}