using Unify.Domain.Abstractions;
using Unify.Domain.Shared;

namespace Unify.Domain.UniversityCore;

public sealed class Specialization : Entity
{

    private Specialization() { }
    public Specialization(Guid id, Name name, Description description, Guid fieldOfStudy) : base(id)
    {
        Name = name;
        Description = description;
        FieldOfStudyId = fieldOfStudy;
    }

    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public Guid FieldOfStudyId { get; private set; }

    public void Update(Name name, Description description)
    {
        Name = name;
        Description = description;
    }
}