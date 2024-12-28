using Dapper;
using Unify.Application.Abstractions.Data;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Users.GetLoggedInUser;
using Unify.Domain.Abstractions;
using Unify.Domain.Users;

namespace Unify.Application.Users.GetAllUsers;

public sealed class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, List<UsersResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(ISqlConnectionFactory sqlConnectionFactory, IUserRepository userRepository)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _userRepository = userRepository;
    }

    public async Task<Result<List<UsersResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        if(!users.Any())
        {
            return Result.Failure<List<UsersResponse>>(Error.NullValue);
        }



        return Result.Success(users.Select(u =>
            new UsersResponse(u.Id.ToString(), u.FirstName.Value, u.LastName.Value, u.Email.Value, u.Roles.Select(r => r.Name).ToList())).ToList());
    }
}
