using Unify.Domain.UniversityCore;

namespace Unify.Application.StudentGroups;

public record StudentGroupResponse(
    Guid Id,
    string Name,
    Guid SpecializationId,
    int StudyYear,
    int Semester,
    string Term,
    int MaxGroupSize,
    List<Guid> Members,
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
            studentGroup.Members.Select(user => user.Id).ToList(),
            null //TODO
        );
    }
};