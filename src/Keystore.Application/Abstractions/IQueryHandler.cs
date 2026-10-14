using Keystore.Domain.Abstractions;
using Keystore.Domain.Common;
using Mediator;

namespace Keystore.Application.Abstractions;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>> where TQuery : IQuery<TResponse> { }