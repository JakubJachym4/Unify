using Unify.Domain.Abstractions;

namespace Unify.Domain.UniversityClasses;

public static class ClassOfferingErrors
{
    public static Error AlreadyEnrolled(Guid id) =>
        Error.Create("ClassOffering.AlreadyEnrolled",
            "The user with the specified identifier is already enrolled into this clases. Id: {0}",
            id);
    public static Error ClassFull(int size) =>
        Error.Create("ClassOffering.ClassFull",
            "The class is already full. Max size of this class is: {0}",
            size);
    public static Error InvalidGroup() =>
        Error.Create("ClassOffering.InvalidGroup",
            "The class is bound to specific group. User must be member of said group to join class.");
}