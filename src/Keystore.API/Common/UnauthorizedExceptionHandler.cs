using Keystore.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace Keystore.API.Common;

/// <summary>
/// Maps <see cref="UnauthorizedAccessException"/> to a 401, e.g. when a required claim is missing from the JWT.
/// These are thrown in the endpoints before the MediatR pipeline runs, so <c>ExceptionBehavior</c> never sees them.
/// </summary>
internal sealed class UnauthorizedExceptionHandler(ILogger<UnauthorizedExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not UnauthorizedAccessException) return false;

        logger.LogWarning("Rejected request to {Path}: {Message}", httpContext.Request.Path, exception.Message);

        var type = ErrorType.Unauthorized;
        await Results.Problem(
                statusCode: type.ToHttpStatusCode(),
                type: type.ToProblemDetailsTypeUri(),
                title: type.ToProblemDetailsTitle(),
                detail: exception.Message)
            .ExecuteAsync(httpContext);

        return true;
    }
}
