using Unify.Domain.Abstractions;

namespace Unify.Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid id) =>
        Error.Create("User.NotFound",
            "The user with the specified identifier was not found. Id: {0}",
            id);

    public static Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "The provided credentials were invalid");

    public static Error AlreadyExists = new(
        "User.AlreadyExists",
        "This user already exists");

    public static Error AlreadyLoggedIn = new(
        "User.AlreadyLoggedIn",
        "Can't register when logged in");
}