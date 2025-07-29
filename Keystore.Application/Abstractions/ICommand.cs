using Keystore.Domain.Abstractions;
using Keystore.Domain.Common;
using MediatR;

namespace Keystore.Application.Abstractions;

public interface ICommand : IRequest<Result> { }
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }