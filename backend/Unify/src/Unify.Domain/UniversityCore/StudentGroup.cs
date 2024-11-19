using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.Users;

namespace Unify.Domain.UniversityCore;

public sealed class StudentGroup : Entity
{
    private readonly List<User> _members = new();
    private readonly List<ClassEnrollment> _classEnrollments = new();

    public Name Name { get; private set; }
    public Guid SpecializationId { get; private set; }
    public StudyYear StudyYear { get; private set; }
    public Semester Semester { get; private set; }
    public Term Term { get; private set; }
    public int MaxGroupSize {get; private set;}
    public void IncrementStudyYear() => StudyYear = new StudyYear(StudyYear.StartingYear + 1);
    public void SetGroupSize(int size) => MaxGroupSize = size;
    public void ChangeTerm()
    {
        if (Term == Term.Summer)
        {
            Term = Term.Winter;
        }
        else
        {
            Term = Term.Summer;
        }
    }


    public IReadOnlyCollection<User> Members => _members;
    public IReadOnlyCollection<ClassEnrollment> ClassEnrollments => _classEnrollments;

    public Result Join(User user)
    {
        if (_members.Any(student => student.Id == user.Id))
        {
            return Result.Failure(StudentGroupErrors.AlreadyEnrolled(user.Id));
        }

        if (_members.Count == MaxGroupSize)
        {
            return Result.Failure(StudentGroupErrors.GroupFull(MaxGroupSize));
        }

        _members.Add(user);
        return Result.Success();
    }

}