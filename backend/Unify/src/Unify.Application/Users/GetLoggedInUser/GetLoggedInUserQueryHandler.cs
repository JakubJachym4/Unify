using Unify.Domain.Abstractions;
using Dapper;
using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Data;
using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Users.GetLoggedInUser;

internal sealed class GetLoggedInUserQueryHandler
    : IQueryHandler<GetLoggedInUserQuery, UserResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IUserContext _userContext;
    public GetLoggedInUserQueryHandler(
        ISqlConnectionFactory sqlConnectionFactory,
        IUserContext userContext)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _userContext = userContext;
    }

    public async Task<Result<UserResponse>> Handle(
        GetLoggedInUserQuery request,
        CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                id AS Id,
                first_name AS FirstName,
                last_name AS LastName,
                email AS Email
            FROM users
            WHERE identity_id = @IdentityId
            """;

        var user = await connection.QuerySingleAsync<UserResponse?>(
            sql,
            new
            {
                _userContext.IdentityId
            });

        if(user == null)
        {
            return Result.Failure<UserResponse>(Error.NullValue);
        }

        const string rolesSql = """
                                SELECT r.name AS RoleName
                                FROM roles r
                                INNER JOIN role_user ru ON r.id = ru.roles_id
                                WHERE ru.users_id = @UserId
                                """;

        var roles = await connection.QueryAsync<string>(
            rolesSql,
            new { UserId = user.Id });

        user.Roles = roles.ToList();

        return user;
    }
}