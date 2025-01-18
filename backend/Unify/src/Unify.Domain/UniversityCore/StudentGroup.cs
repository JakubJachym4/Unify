using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore.Errors;
using Unify.Domain.Users;

namespace Unify.Domain.UniversityCore;

public sealed class StudentGroup : Entity
{
    private StudentGroup()
    {
    }

    private readonly List<User> _members = new();

    private readonly List<ClassEnrollment> _classEnrollments = new();

    private StudentGroup(Name name, Guid specializationId, StudyYear studyYear, Semester semester, Term term, int maxGroupSize) : base(Guid.NewGuid())
    {
        Name = name;
        SpecializationId = specializationId;
        StudyYear = studyYear;
        Semester = semester;
        Term = term;
        MaxGroupSize = maxGroupSize;
    }

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

    public void Update(Name name, StudyYear studyYear, Semester semester)
    {
        Name = name;
        StudyYear = studyYear;
        Semester = semester;
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

    public Result Leave(User user)
    {
        var studentToRemove = _members.FirstOrDefault(s => s.Id == user.Id);
        if (studentToRemove == null)
        {
            return Result.Failure(StudentGroupErrors.NotEnrolled);
        }

        _members.Remove(studentToRemove);
        return Result.Success();
    }

    public static List<StudentGroup> CreateMultiple(Name name, Specialization specialization, StudyYear studyYear, Semester semester, Term term, int combinedSize, int maxGroupSize)
    {
        var groups = new List<StudentGroup>();
        var groupCount = combinedSize / maxGroupSize;
        if(combinedSize % maxGroupSize != 0)
        {
            groupCount++;
        }
        for(int i = 0; i < groupCount; i++)
        {
            groups.Add(new StudentGroup(new Name($"{name.Value}{i+1}"), specialization.Id, studyYear, semester, term, maxGroupSize));
        }

        return groups;
    }

    public static StudentGroup Create(Name name, Specialization specialization, StudyYear studyYear, Semester semester, Term term, int maxGroupSize)
    {
        return new StudentGroup(name, specialization.Id, studyYear, semester, term, maxGroupSize);
    }

    public void Update(Name name,  StudyYear studyYear, Semester semester, Term term, int maxGroupSize)
    {
        Name = name;
        StudyYear = studyYear;
        Semester = semester;
        Term = term;
        MaxGroupSize = maxGroupSize;
    }
    public void ChangeSpecialization(Specialization specialization)
    {
        SpecializationId = specialization.Id;
    }

}