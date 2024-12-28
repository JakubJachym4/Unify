namespace Unify.Application.Users.GetAllUsers;

public sealed record UsersResponse(string Id, string FirstName, string LastName, string Email, List<string> Roles);