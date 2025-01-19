using Unify.Domain.UniversityClasses;

namespace Unify.Application.ClassOfferingSessions;

public record ClassOfferingSessionResponse(Guid Id, Guid ClassOfferingId, string Title, DateTime ScheduledDate, TimeSpan Duration, Guid LecturerId, Guid LocationId)
{
    public static ClassOfferingSessionResponse CreateFrom(ClassOfferingSession session)
    {
        return new ClassOfferingSessionResponse(session.Id, session.ClassOfferingId, session.Title.Value, session.ScheduledDate, session.Duration, session.LecturerId, session.LocationId);
    }
}