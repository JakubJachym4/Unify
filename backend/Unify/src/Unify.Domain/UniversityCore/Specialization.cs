using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.Users;

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


    private readonly List<User> _students = new();
    public IReadOnlyCollection<User> Students => _students;

    public void Update(Name name, Description description)
    {
        Name = name;
        Description = description;
    }

    public void AssignStudent(User student)
    {
        if(_students.All(s => s.Id != student.Id))
            _students.Add(student);
    }

    public void UnassignStudent(User student)
    {
        var studentToRemove = _students.FirstOrDefault(s => s.Id == student.Id);
        if(studentToRemove != null)
            _students.Remove(studentToRemove);
    }

}