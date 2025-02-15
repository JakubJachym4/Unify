using Unify.Domain.Users;

namespace Unify.Application.Users.GetLoggedInUser;

public sealed class UserResponse
{
    public Guid Id { get; init; }

    public string Email { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }

    public List<string> Roles { get; set; }

    public static List<UserResponse> FromUsers(IEnumerable<User?> users)
    {
        return users.Where(u => u != null).Select(u => new UserResponse
        {
            Id = u!.Id,
            Email = u.Email.Value,
            FirstName = u.FirstName.Value,
            LastName = u.LastName.Value,
            Roles = u.Roles.Select(r => r.Name).ToList()
        }).ToList();
    }
}