using Unify.Domain.Abstractions;
using MediatR;

namespace Unify.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}