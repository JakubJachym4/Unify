using Unify.Domain.Messages;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.Users;

namespace Unify.Domain.UniversityClasses;

public sealed class OfferingSession : ClassSession
{
    public OfferingSession(ClassOffering offering, Title title, DateTime scheduledDate, TimeSpan duration, User lecturer, Location location)
        : base(Guid.NewGuid(), title, ClassType.Lecture, scheduledDate, duration, lecturer, location)
    {
        ClassOffering = offering;
    }

    public ClassOffering ClassOffering { get; private set; }
}