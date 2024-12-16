using MediatR;
using Unify.Domain.Abstractions;

namespace Unify.Application.Users.DeleteRole;

public record DeleteRoleCommand(Guid UserId, string Role) : IRequest<Result>;