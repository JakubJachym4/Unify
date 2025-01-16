using Unify.Domain.UniversityCore;

namespace Unify.Application.StudentGroups;

public record StudentGroupResponse(
    Guid Id,
    string Name,
    Guid Specialization,
    int StudyYear,
    int Semester,
    string Term,
    int MaxGroupSize,
    object? ClassOfferingResponse
)
{
    public static StudentGroupResponse CreateFrom(StudentGroup studentGroup)
    {
        return new StudentGroupResponse(
            studentGroup.Id,
            studentGroup.Name.Value,
            studentGroup.SpecializationId,
            studentGroup.StudyYear.StartingYear,
            studentGroup.Semester.Value,
            studentGroup.Term.ToString(),
            studentGroup.MaxGroupSize,
            null //TODO
        );
    }
};