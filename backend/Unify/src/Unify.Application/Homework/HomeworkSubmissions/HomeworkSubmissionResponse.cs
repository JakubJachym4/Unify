using Unify.Application.ClassEnrollment;
using Unify.Application.Files;
using Unify.Domain.OnlineResources;
using Unify.Domain.UniversityCore;

namespace Unify.Application.Homework.HomeworkSubmissions;

public record HomeworkSubmissionResponse(
    Guid Id,
    Guid AssignmentId,
    Guid StudentId,
    MarkResponse? Mark,
    string Feedback,
    DateTime SubmittedOn,
    List<FileResponse>? Attachments)
{
    public HomeworkSubmissionResponse(HomeworkSubmission homeworkSubmission, Mark? mark) : this(
        homeworkSubmission.Id,
        homeworkSubmission.HomeworkAssigmentId,
        homeworkSubmission.StudentId,
        MarkResponse.Create(mark),
        homeworkSubmission.Feedback?.Value ?? string.Empty,
        homeworkSubmission.SubmittedOn,
        FileResponse.FromManyAttachments(homeworkSubmission.Attachments.ToList()))
    {
    }
}