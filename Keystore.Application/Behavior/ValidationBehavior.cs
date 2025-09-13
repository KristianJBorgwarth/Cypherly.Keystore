﻿using FluentValidation;
using Keystore.Domain.Common;
using MediatR;

// ReSharper disable InvertIf

namespace Keystore.Application.Behavior;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validator is null)
            return await next(cancellationToken);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
            return await next(cancellationToken);

        var errorMessage =
            string.Join("; ", validationResult.Errors.Select(e => $"{e.ErrorMessage} ({e.PropertyName})"));

        var error = Error.Validation("Validation failed: " + errorMessage);

        return typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>)
            ? CreateGenericFailResponse(error)
            : CreateFailResponse(error);
    }

    private static TResponse CreateFailResponse(Error error)
    {
        return (TResponse)Result.Fail(error);
    }

    private static TResponse CreateGenericFailResponse(Error error)
    {
        var resultType = typeof(TResponse).GetGenericArguments()[0];
        var method = typeof(Result).GetMethods()
            .FirstOrDefault(m => m is { Name: "Fail", IsGenericMethodDefinition: true })!;

        var genericFailMethod = method.MakeGenericMethod(resultType);
        return (TResponse)genericFailMethod.Invoke(null, [error])!;
    }
}