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

    public static Error GroupNotFound =>
        Error.Create("StudentGroup.NotFound",
            "The group with the specified identifier was not found.");

    public static Error NotEnrolled =>
        Error.Create("StudentGroup.NotEnrolled",
            "The user with the specified identifier is not part of this group.");
}