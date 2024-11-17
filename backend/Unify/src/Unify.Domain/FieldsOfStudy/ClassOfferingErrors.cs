using Unify.Domain.Abstractions;

namespace Unify.Domain.FieldsOfStudy;

public static class ClassOfferingErrors
{
    public static Error AlreadyEnrolled(Guid id) =>
        Error.Create("ClassOffering.AlreadyEnrolled",
            "The user with the specified identifier is already enrolled into this clases. Id: {0}",
            id);
}