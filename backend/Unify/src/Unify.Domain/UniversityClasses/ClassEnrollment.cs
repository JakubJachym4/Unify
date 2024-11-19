using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;
using Guid = System.Guid;

namespace Unify.Domain.UniversityClasses;

public sealed class ClassEnrollment : Entity
{
    public Guid ClassOfferingId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid StudentGroupId { get; private set; }
    public DateTime EnrollmentOn { get; private set; }


    private ClassEnrollment(Guid id, Guid classOfferingId, Guid studentId, DateTime enrollmentOn) : base(id)
    {
        ClassOfferingId = classOfferingId;
        StudentId = studentId;
        EnrollmentOn = enrollmentOn;
    }

    public static ClassEnrollment Enroll(ClassOffering classOffering, User student, DateTime enrollmentOn)
    {
        return new ClassEnrollment(
            Guid.NewGuid(),
            classOffering.Id,
            student.Id,
            enrollmentOn);
    }

    public Grade AddGrade(Description description)
    {
        return Grade.Create(this, description);
    }

    public Grade AddGrade(Description description, Score score, DateTime dateAwarded)
    {
        return Grade.Create(this, description, score, dateAwarded);
    }
}