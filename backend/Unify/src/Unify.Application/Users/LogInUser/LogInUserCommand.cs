using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Users.LogInUser;

public sealed record LogInUserCommand(string Email, string Password)
    : ICommand<AccessTokenResponse>;