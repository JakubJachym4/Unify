using Unify.Domain.Abstractions;

namespace Unify.Domain.UniversityCore.Errors;

public static class StudentGroupErrors
{
    public static Error AlreadyEnrolled(Guid id) =>
        Error.Create("StudentGroup.AlreadyEnrolled",
            "The user with the specified identifier is already part of this group. Id: {0}",
            id);

    public static Error GroupFull(int size) =>
        Error.Create("StudentGroup.ClassFull",
            "The group is already full. Max size of this group is: {0}",
            size);

    public static Error NotFound =>
        Error.Create("StudentGroup.NotFound",
            "The group with the specified identifier was not found.");

    public static Error NotEnrolled =>
        Error.Create("StudentGroup.NotEnrolled",
            "The user with the specified identifier is not part of this group.");

    public static Error NotEmpty =>
        Error.Create("StudentGroup.NotEmpty",
            "The group is not empty. Remove all members before deleting the group.");

    public static Error UserPresentInSpecializationGroup =>
        Error.Create("StudentGroup.UserPresentInGroup",
            "User is present in group. Before removing user from specialization, remove user from group.");

    public static Error CannotAssignStudent =>
        Error.Create("StudentGroup.CannotAssignStudent",
            "Cannot assign student to group. Group is full or student is already assigned to another group.");

}