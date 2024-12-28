using MediatR;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;

namespace Unify.Application.Users.RemoveRole;

public record RemoveRoleCommand(Guid UserId, string Role) : ICommand<Result>;