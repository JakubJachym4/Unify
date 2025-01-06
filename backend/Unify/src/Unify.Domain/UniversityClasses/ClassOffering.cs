using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;
using Guid = System.Guid;

namespace Unify.Domain.UniversityClasses;

public sealed class ClassOffering : Entity
{
    private readonly List<ClassEnrollment> _enrollments = new();
    public IReadOnlyCollection<ClassEnrollment> Enrollments => _enrollments;


    public Name Name { get; private set; }
    public Guid CourseId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public Guid LecturerId { get; private set; }
    public Guid? BoundGroupId { get; private set; }

    public int MaxStudentsCount { get; private set; }


    private ClassOffering() {}

    private ClassOffering(Guid id, Name name, Guid courseId, DateOnly startDate, DateOnly endDate, Guid lecturerId, Guid? boundGroupId, int maxStudentsCount) : base(id)
    {
        Name = name;
        CourseId = courseId;
        StartDate = startDate;
        EndDate = endDate;
        LecturerId = lecturerId;
        BoundGroupId = boundGroupId;
        MaxStudentsCount = maxStudentsCount;
    }

    public static ClassOffering Create(Name name, Course course, DateOnly startDate, DateOnly endDate, User lecturer, StudentGroup? studentGroup,
        int maxStudentsCount)
    {
        return new ClassOffering(
            Guid.NewGuid(),
            name,
            course.Id,
            startDate,
            endDate,
            lecturer.Id,
            studentGroup?.Id,
            maxStudentsCount);
    }

    public void Update(Name name, DateOnly startDate, DateOnly endDate, int maxStudentsCount)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        MaxStudentsCount = maxStudentsCount;
    }

    public void SetMaxStudentsCount(int maxStudentsCount) => MaxStudentsCount = maxStudentsCount;

    public Result Enroll(User student, DateTime enrollmentDate, StudentGroup? boundGroup = null)
    {
        if (BoundGroupId != null && BoundGroupId != boundGroup?.Id)
        {
            return Result.Failure(ClassOfferingErrors.InvalidGroup());
        }

        if (_enrollments.Any(e => e.StudentId == student.Id))
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