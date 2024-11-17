using Unify.Domain.Abstractions;
using Unify.Domain.Shared;

namespace Unify.Domain.UniversityCore;

public sealed class Specialization : Entity
{
    private readonly List<StudentGroup> _groups = new();
    private readonly List<Course> _courses = new();

    public Specialization(Guid id, Name name, Description description, FieldOfStudy fieldOfStudy) : base(id)
    {
        Name = name;
        Description = description;
        FieldOfStudy = fieldOfStudy;
    }

    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public FieldOfStudy FieldOfStudy { get; private set; }

    public IReadOnlyCollection<StudentGroup> Groups => _groups;
    public IReadOnlyCollection<Course> Courses => _courses;

    public void AddGroup(StudentGroup group) => _groups.Add(group);
    public void AddCourse(Course course) => _courses.Add(course);
}