using Unify.Domain.Abstractions;
using Unify.Domain.Users;

namespace Unify.Domain.FieldsOfStudy;

public sealed class ClassEnrollment : Entity
{
    public ClassOffering ClassOffering { get; private set; }
    public User Student { get; private set; }
    public DateTime EnrollmentOn { get; private set; }

    private readonly List<Grade> _grades = new();
    public IReadOnlyCollection<Grade> Grades => _grades;

    private ClassEnrollment(Guid id, ClassOffering classOffering, User student, DateTime enrollmentOn) : base(id)
    {
        ClassOffering = classOffering;
        Student = student;
        EnrollmentOn = enrollmentOn;
    }

    public static ClassEnrollment Enroll(ClassOffering classOffering, User student, DateTime enrollmentOn)
    {
        return new ClassEnrollment(
            Guid.NewGuid(),
            classOffering,
            student,
            enrollmentOn);
    }

    public void AddGrade(Description description)
    {
        _grades.Add(new Grade(description));
    }

    public void AddGrade(Description description, Score score, DateTime dateAwarded)
    {
        _grades.Add(new Grade(description, score, dateAwarded));
    }
}