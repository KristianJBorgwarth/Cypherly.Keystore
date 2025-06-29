using Cypherly.Keystore.Domain.Common.Result;
using MediatR;

namespace Cypherly.Keystore.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }