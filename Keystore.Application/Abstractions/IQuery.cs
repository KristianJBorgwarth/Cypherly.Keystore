using Keystore.Domain.Abstractions;
using Keystore.Domain.Common;
using MediatR;

namespace Keystore.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }