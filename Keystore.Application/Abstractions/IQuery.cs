using Keystore.Domain.Abstractions;
using MediatR;

namespace Keystore.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }