using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;

namespace Unify.Domain.UniversityClasses;

public sealed class Lecture : ClassSession
{
    private Lecture(){}
    public Lecture(Course course, Title title, DateTime scheduledDate, TimeSpan duration, User lecturer, Location location)
        : base(Guid.NewGuid(), title, ClassType.Lecture, scheduledDate, duration, lecturer, location)
    {
        CourseId = course.Id;
    }

    public Guid CourseId { get; private set; }
}