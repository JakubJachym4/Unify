using Unify.Domain.Abstractions;
using Unify.Domain.OnlineResources;
using Unify.Domain.Shared;

namespace Unify.Domain.UniversityCore;

public sealed class Mark : Entity
{
    private Mark() { }
    private Mark(Guid gradeId, Guid? submissionId, Score score, Score maxScore, DateTime dateAwarded, bool homeworkMark, Description? criteria = null) : base(Guid.NewGuid())
    {
        GradeId = gradeId;
        SubmissionId = submissionId;
        Criteria = criteria;
        Score = score;
        MaxScore = maxScore;
        DateAwarded = dateAwarded;
        HomeworkMark = homeworkMark;
    }
    public Guid GradeId { get; private set; }
    public Guid? SubmissionId { get; private set; }

    public Description? Criteria  { get; private set; }
    public Score Score { get; private set; }
    public Score MaxScore { get; private set; }

    public DateTime DateAwarded { get; private set; }
    public bool HomeworkMark { get; private set; }


    public static Mark CreateForSubmission(Grade grade, HomeworkSubmission submission, Score score, Score maxScore, DateTime dateAwarded, Description? criteria = null)
    {
        var mark = Create(grade.Id, submission.Id, score, maxScore, dateAwarded, true, criteria);
        submission.SetMark(mark);
        return mark;
    }

    public static Mark CreateForGrade(Grade grade, Score score, Score maxScore, DateTime dateAwarded, Description? criteria = null)
    {
        return Create(grade.Id, null, score, maxScore, dateAwarded, false, criteria);
    }

    private static Mark Create(Guid gradeId, Guid? submissionId, Score score, Score maxScore, DateTime dateAwarded, bool homeworkMark, Description? criteria = null)
    {
        if (score.Value > maxScore.Value || score.Value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(score), $"Score must be between 0 and {maxScore.Value}.");
        }

        return new Mark(gradeId, submissionId, score, maxScore, dateAwarded, homeworkMark, criteria);
    }

}