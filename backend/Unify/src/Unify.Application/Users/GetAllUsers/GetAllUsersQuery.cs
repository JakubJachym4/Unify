using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.Users;

namespace Unify.Application.Users.GetAllUsers;

public record GetAllUsersQuery : IQuery<List<UsersResponse>>;