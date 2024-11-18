using Unify.Domain.Abstractions;
using Unify.Domain.Shared;

namespace Unify.Domain.UniversityCore;

public sealed class Grade : Entity
{
    public Description Description { get; private set; }
    public Score? Score { get; private set; }
    public DateTime? DateAwarded { get; private set; }

    private Grade() { }

    public Grade(Description description, Score? score = null, DateTime? dateAwarded = null) : base(Guid.NewGuid())
    {
        Description = description;
        Score = score;
    }

    private readonly List<Mark> _marks = new();
    public IReadOnlyCollection<Mark> Marks => _marks;

    public void AddMark(Mark mark)
    {
        if (DateAwarded != null)
        {
            throw new InvalidOperationException("Grade have already been awarded.");
        }

        _marks.Add(mark);
        RecalculateScore();
    }

    public void SetDateAwarded(DateTime dateAwarded) => DateAwarded = dateAwarded;
    public void RevokeGradeAwarding() => DateAwarded = null;
    private void RecalculateScore() => Score = _marks.Average(m => m.Score.Value / m.MaxScore.Value * 100);

}