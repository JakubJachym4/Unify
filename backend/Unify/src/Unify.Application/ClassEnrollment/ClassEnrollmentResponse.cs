namespace Unify.Application.ClassEnrollment;

public record ClassEnrollmentResponse(Guid ClassOfferingId, Guid StudentId, DateTime EnrolledOn)
{
    public static ClassEnrollmentResponse CreateFrom(Domain.UniversityClasses.ClassEnrollment enrollment)
    {
        return new ClassEnrollmentResponse(enrollment.ClassOfferingId, enrollment.StudentId, enrollment.EnrolledOn);
    }
}