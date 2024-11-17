namespace Unify.Domain.UniversityCore;

public sealed record StudyYear
{
    public int StartingYear { get; private set; }
    public int EndingYear { get; private set; }

    public StudyYear(int startingYear)
    {
        StartingYear = startingYear;
        EndingYear = startingYear + 1;
    }

    public override string ToString()
    {
        return StartingYear + "/" + EndingYear;
    }
}