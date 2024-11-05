using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Users.LogOutUser;

public sealed record LogOutUserCommand(string Token) : ICommand;