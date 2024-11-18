using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;

namespace Unify.Domain.UniversityClasses;

public sealed class ClassOffering : Entity
{
    private readonly List<ClassEnrollment> _enrollments = new();
    private readonly List<Message> _messages = new();
    public IReadOnlyCollection<ClassEnrollment> Enrollments => _enrollments;
    public IReadOnlyCollection<Message> Messages => _messages;

    public Name Name { get; private set; }
    public Course Course { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public User Lecturer { get; private set; }
    public StudentGroup? BoundGroup { get; private set; }

    public int MaxStudentsCount { get; private set; }

    public ClassOffering(Name name, Course course, DateOnly startDate, DateOnly endDate, User lecturer, int maxStudentsCount, StudentGroup? boundGroup = null) : base(Guid.NewGuid())
    {
        Name = name;
        Course = course;
        StartDate = startDate;
        EndDate = endDate;
        Lecturer = lecturer;
        MaxStudentsCount = maxStudentsCount;
        BoundGroup = boundGroup;
    }

    public void SetMaxStudentsCount(int maxStudentsCount) => MaxStudentsCount = maxStudentsCount;

    public Result Enroll(User student, DateTime enrollmentDate, StudentGroup? boundGroup = null)
    {
        if (BoundGroup != null && BoundGroup.Id != boundGroup?.Id)
        {
            return Result.Failure(ClassOfferingErrors.InvalidGroup());
        }

        if (_enrollments.Any(e => e.Student.Id == student.Id))
        {
            return Result.Failure(ClassOfferingErrors.AlreadyEnrolled(student.Id));
        }

        if (_enrollments.Count == MaxStudentsCount)
        {
            return Result.Failure(ClassOfferingErrors.ClassFull(MaxStudentsCount));
        }

        _enrollments.Add(ClassEnrollment.Enroll(this, student, enrollmentDate));

        return Result.Success();
    }
}