using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;

namespace Unify.Domain.UniversityClasses;

public abstract class ClassSession : Entity
{
    protected ClassSession(){}

    protected ClassSession(Guid id, Title title, ClassType classType, DateTime scheduledDate, TimeSpan duration, User lecturer, Location location) : base(id)
    {
        Title = title;
        ClassType = classType;
        ScheduledDate = scheduledDate;
        Duration = duration;
        LecturerId = lecturer.Id;
        LocationId = location.Id;
    }


    public Title Title { get; private set; }
    public ClassType ClassType { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public TimeSpan Duration { get; private set; }
    public Guid LecturerId { get; private set; }
    public Guid LocationId { get; private set; }
}