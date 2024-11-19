using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;

namespace Unify.Domain.UniversityClasses;

public sealed class OfferingSession : ClassSession
{
    private OfferingSession(){}
    public OfferingSession(ClassOffering offering, Title title, DateTime scheduledDate, TimeSpan duration, User lecturer, Location guid)
        : base(Guid.NewGuid(), title, ClassType.Lecture, scheduledDate, duration, lecturer, guid)
    {
        ClassOfferingId = offering.Id;
    }

    public Guid ClassOfferingId { get; private set; }
}