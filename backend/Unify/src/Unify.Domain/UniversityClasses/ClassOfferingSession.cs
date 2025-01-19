using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;

namespace Unify.Domain.UniversityClasses;

public sealed class ClassOfferingSession : ClassSession
{
    private ClassOfferingSession(){}
    public ClassOfferingSession(ClassOffering offering, Title title, DateTime scheduledDate, TimeSpan duration, User lecturer, Location guid)
        : base(Guid.NewGuid(), title, ClassType.Lecture, scheduledDate, duration, lecturer, guid)
    {
        ClassOfferingId = offering.Id;
    }

    public Guid ClassOfferingId { get; private set; }

    public void Update(Title title, DateTime requestScheduledDate, TimeSpan requestDuration, Guid requestLecturerId, Guid requestLocationId)
    {
        Title = title;
        ScheduledDate = requestScheduledDate;
        Duration = requestDuration;
        LecturerId = requestLecturerId;
        LocationId = requestLocationId;
    }
}