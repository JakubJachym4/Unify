using Unify.Domain.Abstractions;

namespace Unify.Domain.Users;

public static class UserErrors
{
    public static Error NotFound = new(
        "User.NotFound",
        "The user with the specified identifier was not found");

    public static Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "The provided credentials were invalid");

    public static Error AlreadyExists = new(
        "User.AlreadyExists",
        "This user already exists");
}