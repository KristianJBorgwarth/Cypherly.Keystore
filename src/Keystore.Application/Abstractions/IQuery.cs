using Keystore.Domain.Abstractions;
using Keystore.Domain.Common;
using Mediator;

namespace Keystore.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }