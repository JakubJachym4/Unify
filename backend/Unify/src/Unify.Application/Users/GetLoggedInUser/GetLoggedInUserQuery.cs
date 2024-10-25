using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Users.GetLoggedInUser;

public sealed record GetLoggedInUserQuery : IQuery<UserResponse>;