using Cypherly.Keystore.Domain.Abstractions;
using MediatR;

namespace Cypherly.Keystore.Application.Abstractions;

public interface ICommand : IRequest<Result> { }
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }