using Unify.Application.Files;
using Unify.Domain.OnlineResources;

namespace Unify.Application.Homework.HomeworkSubmissions;

public record HomeworkSubmissionResponse(
    Guid Id,
    Guid AssignmentId,
    Guid StudentId,
    Guid? MarkId,
    string Feedback,
    DateTime SubmittedOn,
    List<FileResponse>? Attachments)
{
    public HomeworkSubmissionResponse(HomeworkSubmission homeworkSubmission) : this(
        homeworkSubmission.Id,
        homeworkSubmission.HomeworkAssigmentId,
        homeworkSubmission.StudentId,
        homeworkSubmission.MarkId,
        homeworkSubmission.Feedback?.Value ?? string.Empty,
        homeworkSubmission.SubmittedOn,
        FileResponse.FromManyAttachments(homeworkSubmission.Attachments.ToList()))
    {
    }
}