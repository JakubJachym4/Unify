using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.Users;

namespace Unify.Domain.UniversityCore;

public sealed class Course : Entity
{

    private Course() { }
    private Course(Name name, Description description, Guid specializationId) : base(Guid.NewGuid())
    {
        Name = name;
        Description = description;
        SpecializationId = specializationId;
    }

    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public Guid SpecializationId { get; private set; }
    public Guid? LecturerId { get; private set; }

    private readonly List<ClassOffering> _classes = new();
    public IReadOnlyCollection<ClassOffering> Classes => _classes;


    public static Course Create(Name name, Description description, Specialization specialization)
    {
        return new Course(name, description, specialization.Id);
    }

    public void AddClass(ClassOffering offering) => _classes.Add(offering);
    public void AssignLecturer(User lecturer) => LecturerId = lecturer.Id;

    public void Update(Name name, Description description)
    {
        Name = name;
        Description = description;
    }
}