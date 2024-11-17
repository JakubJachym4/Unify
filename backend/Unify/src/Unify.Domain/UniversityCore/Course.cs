using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;

namespace Unify.Domain.UniversityCore;

public sealed class Course : Entity
{
    public Course(Name name, Description description, Specialization specialization) : base(Guid.NewGuid())
    {
        Name = name;
        Description = description;
        Specialization = specialization;
    }

    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public Specialization Specialization { get; private set; }

    private readonly List<ClassOffering> _classes = new();
    public IReadOnlyCollection<ClassOffering> Classes => _classes;

    public void AddClass(ClassOffering offering) => _classes.Add(offering);
}