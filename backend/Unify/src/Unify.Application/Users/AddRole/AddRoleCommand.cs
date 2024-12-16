using MediatR;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;

namespace Unify.Application.Users.AddRole;

public record AddRoleCommand(Guid UserId, string Role) : ICommand;