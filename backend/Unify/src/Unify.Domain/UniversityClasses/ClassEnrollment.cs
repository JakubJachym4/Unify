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
    public DateTime EnrolledOn { get; private set; }
    public Guid GradeId { get; private set; }


    private ClassEnrollment(Guid id, Guid classOfferingId, Guid studentId, DateTime enrolledOn) : base(id)
    {
        ClassOfferingId = classOfferingId;
        StudentId = studentId;
        EnrolledOn = enrolledOn;
    }

    public static ClassEnrollment Enroll(ClassOffering classOffering, User student, DateTime enrollmentOn)
    {

        var enrollment = new ClassEnrollment(
            Guid.NewGuid(),
            classOffering.Id,
            student.Id,
            enrollmentOn);

        enrollment.CreateGrade(new Description(string.Empty));

        return enrollment;
    }

    private void CreateGrade(Description description)
    {
        var grade = Grade.Create(description);
        GradeId = grade.Id;
    }

}