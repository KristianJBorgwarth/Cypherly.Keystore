using Cypherly.Keystore.Domain.Abstractions;
using MediatR;

namespace Cypherly.Keystore.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }