namespace Unify.Domain.Users.Extensions;

public static class UserExtensions
{
    public static bool AllFound(this ICollection<User> users, ICollection<Guid> guidsLookedFor)
    {
        return guidsLookedFor.All(x => users.Any(u => u.Id == x));
    }
}