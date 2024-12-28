namespace Unify.Domain.Users;

public sealed class Role
{
    public static readonly Role Registered = new(1, "Registered");
    public static readonly Role Administrator = new(2, "Administrator");
    public static readonly Role Student = new(3, "Student");
    public static readonly Role Lecturer = new(4, "Lecturer");

    public static readonly Role[] All = [Registered, Administrator, Student, Lecturer];

    private Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Role? GetByName(string name)
    {
        return All.FirstOrDefault(role => role.Name == name);
    }

    public int Id { get; init; }

    public string Name { get; init; }


    //TODO: remove ROLE.USER_ID from entity
    public ICollection<User> Users { get; init; } = new List<User>();

    public ICollection<Permission> Permissions { get; init; } = new List<Permission>();
}
