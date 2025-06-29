using Cypherly.Keystore.Domain.Abstractions;
using MediatR;

namespace Cypherly.Keystore.Application.Abstractions;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>> where TQuery : IQuery<TResponse> { }