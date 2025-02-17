using Unify.Domain.Abstractions;

namespace Unify.Domain.OnlineResources.Errors;

public static class HomeworkSubmissionErrors
{
    public static Error NotFound =>
        new("HomeworkSubmission.NotFound", "Homework submission not found.");

    public static Error AlreadySubmitted =>
        new("HomeworkSubmission.AlreadySubmitted", "Already submitted.");

    public static Error AlreadyGraded =>
        new("HomeworkSubmission.AlreadyGraded", "Submission has been graded and cannot be changed.");
}