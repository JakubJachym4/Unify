using Unify.Domain.Abstractions;
using Unify.Domain.Shared;

namespace Unify.Domain.UniversityCore;

public sealed class FieldOfStudy : Entity
{
    public FieldOfStudy(Guid id, Name name, Description description, Guid facultyId) : base(id)
    {
        Name = name;
        Description = description;
        FacultyId = facultyId;
    }

    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public Guid FacultyId { get; private set; }
}