using Keystore.Domain.Abstractions;
using FluentValidation;
using Keystore.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

// ReSharper disable InvertIf

namespace Keystore.Application.Behavior;

public class ValidationBehavior<TRequest, TResponse>(
    ILogger<ValidationBehavior<TRequest, TResponse>> logger,
    IValidator<TRequest>? validator = null) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validator is null)
            return await next(cancellationToken);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
            return await next(cancellationToken);

        var errorMessage = string.Join("; ", validationResult.Errors.Select(e => $"{e.ErrorMessage} ({e.PropertyName})"));

        var error = Error.Validation("Validation failed: " + errorMessage);

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            logger.LogWarning("Validation failed: {ErrorMessage}", errorMessage);

            var genericType = typeof(TResponse).GetGenericArguments()[0];
            var failMethod = typeof(Result)
                .GetMethod(nameof(Result.Fail))!
                .MakeGenericMethod(genericType);

            return (TResponse)failMethod.Invoke(null, [error])!;
        }

        return (TResponse)Result.Fail(error);

    }
}