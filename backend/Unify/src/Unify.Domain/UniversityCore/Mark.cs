using Unify.Domain.Abstractions;
using Unify.Domain.OnlineResources;
using Unify.Domain.Shared;

namespace Unify.Domain.UniversityCore;

public sealed class Mark : Entity
{
    private Mark() { }
    private Mark(Guid gradeId, Guid? submissionId, Title title, Score score, Score maxScore, DateTime dateAwarded, bool homeworkMark) : base(Guid.NewGuid())
    {
        GradeId = gradeId;
        SubmissionId = submissionId;
        Title = title;
        Score = score;
        MaxScore = maxScore;
        DateAwarded = dateAwarded;
        HomeworkMark = homeworkMark;
    }
    public Title Title { get; private set; }
    public Guid GradeId { get; private set; }
    public Guid? SubmissionId { get; private set; }
    public Score Score { get; private set; }
    public Score MaxScore { get; private set; }

    public DateTime DateAwarded { get; private set; }
    public bool HomeworkMark { get; private set; }


    public static Mark CreateForSubmission(Title title, Grade grade, HomeworkSubmission submission, Score score, Score maxScore, DateTime dateAwarded)
    {
        var mark = Create(grade.Id, submission.Id, title, score, maxScore, dateAwarded, true);
        submission.SetMark(mark);
        return mark;
    }

    public static Mark CreateForGrade(Title title, Grade grade, Score score, Score maxScore, DateTime dateAwarded)
    {
        return Create(grade.Id, null, title, score, maxScore, dateAwarded, false);
    }

    private static Mark Create(Guid gradeId, Guid? submissionId, Title title, Score score, Score maxScore, DateTime dateAwarded, bool homeworkMark)
    {
        if (score.Value > maxScore.Value || score.Value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(score), $"Score must be between 0 and {maxScore.Value}.");
        }

        return new Mark(gradeId, submissionId, title, score, maxScore, dateAwarded, homeworkMark);
    }

}