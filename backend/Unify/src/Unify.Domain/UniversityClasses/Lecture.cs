using Unify.Domain.Messages;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;

namespace Unify.Domain.UniversityClasses;

public sealed class Lecture : ClassSession
{
    public Lecture(Course course, Title title, DateTime scheduledDate, TimeSpan duration, User lecturer, Location location)
        : base(Guid.NewGuid(), title, ClassType.Lecture, scheduledDate, duration, lecturer, location)
    {
        Course = course;
    }

    public Course Course { get; private set; }
}