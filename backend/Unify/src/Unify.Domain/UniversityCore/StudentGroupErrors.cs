using Unify.Domain.Abstractions;

namespace Unify.Domain.UniversityCore;

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
}