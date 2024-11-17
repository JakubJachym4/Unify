using Unify.Domain.Abstractions;
using Unify.Domain.Shared;

namespace Unify.Domain.UniversityCore;

public sealed class Mark : Entity
{
    private Mark(Description criteria, Score score, Score maxScore) : base(Guid.NewGuid())
    {
        Criteria = criteria;
        Score = score;
        MaxScore = maxScore;
    }
    public Description Criteria  { get; private set; }
    public Score Score { get; private set; }
    public Score MaxScore { get; private set; }


    public static Mark Create(Description criteria, Score score, Score maxScore)
    {
        if (score.Value > maxScore.Value || score.Value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(score), $"Score must be between 0 and {maxScore.Value}.");
        }

        return new Mark(criteria, score, maxScore);
    }

}