using Unify.Domain.UniversityCore;

namespace Unify.Application.ClassEnrollment;

public record ClassEnrollmentResponse(Guid ClassOfferingId, Guid StudentId, DateTime EnrolledOn)
{
    public static ClassEnrollmentResponse CreateFrom(Domain.UniversityClasses.ClassEnrollment enrollment)
    {
        return new ClassEnrollmentResponse(enrollment.ClassOfferingId, enrollment.StudentId, enrollment.EnrolledOn);
    }
}

public record ClassEnrollmentResponseWithGrade(Guid ClassOfferingId, Guid StudentId, DateTime EnrolledOn, GradeResponse? Grade)
{
    public static ClassEnrollmentResponseWithGrade CreateFrom(Domain.UniversityClasses.ClassEnrollment enrollment, Grade? grade)
    {
        var gradeResponse = grade == null ? null : GradeResponse.Create(grade);
        return new ClassEnrollmentResponseWithGrade(enrollment.ClassOfferingId, enrollment.StudentId, enrollment.EnrolledOn,
            gradeResponse);
    }
}

public record GradeResponse(Guid Id, string Description, decimal? Score, DateTime? DateAwarded,  List<MarkResponse> Marks)
{
    public static GradeResponse Create(Grade grade)
    {
        return new GradeResponse(grade.Id, grade.Description.Value, grade.Score?.Value, grade.DateAwarded, grade.Marks.Select(MarkResponse.Create).ToList());
    }
    public static GradeResponse Create(Guid id, string description, decimal score, DateTime dateAwarded, List<MarkResponse> marks)
    {
        return new GradeResponse(id, description, score, dateAwarded, marks);
    }
}

public record MarkResponse(Guid Id, Guid GradeId, Guid? SubmissionId, string? Criteria, decimal Score, decimal MaxScore)
{
    public static MarkResponse Create(Guid id, Guid gradeId, Guid? submissionId, string criteria, decimal score, decimal maxScore)
    {
        return new MarkResponse(id, gradeId, submissionId, criteria, score, maxScore);
    }

    public static MarkResponse Create(Domain.UniversityCore.Mark mark)
    {
        return new MarkResponse(mark.Id, mark.GradeId, mark.SubmissionId, mark.Criteria?.Value, mark.Score.Value, mark.MaxScore.Value);
    }
}
