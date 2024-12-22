using Dapper;
using Unify.Application.Abstractions.Data;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Users.GetLoggedInUser;
using Unify.Domain.Abstractions;
using Unify.Domain.Users;

namespace Unify.Application.Users.GetAllUsers;

public sealed class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, List<User>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(ISqlConnectionFactory sqlConnectionFactory, IUserRepository userRepository)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _userRepository = userRepository;
    }

    public async Task<Result<List<User>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        if(!users.Any())
        {
            return Result.Failure<List<User>>(Error.NullValue);
        }

        return Result.Success(users.ToList());
    }
}
