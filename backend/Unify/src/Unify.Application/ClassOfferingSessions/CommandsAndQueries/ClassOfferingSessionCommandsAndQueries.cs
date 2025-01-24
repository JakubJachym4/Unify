using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.ClassOfferingSessions.CommandsAndQueries;

public record CreateClassOfferingSessionCommand(Guid ClassOfferingId, string Title, DateTime ScheduledDate, TimeSpan Duration, Guid LecturerId, Guid LocationId) : ICommand<Guid>;

public record UpdateClassOfferingSessionCommand(Guid Id, string Title, DateTime ScheduledDate, TimeSpan Duration, Guid LecturerId, Guid LocationId) : ICommand;

public record DeleteClassOfferingSessionCommand(Guid Id) : ICommand;

public record GetClassOfferingSessionQuery(Guid Id) : IQuery<ClassOfferingSessionResponse>;

public record ListClassOfferingSessionsQuery : IQuery<List<ClassOfferingSessionResponse>>;

public record GetSessionByClassOfferingQuery(Guid ClassOfferingId) : IQuery<List<ClassOfferingSessionResponse>>;


public record GetSessionByStudentQuery(Guid StudentId) : IQuery<List<ClassOfferingSessionResponse>>;

public record GetSessionByLecturerQuery(Guid LecturerId) : IQuery<List<ClassOfferingSessionResponse>>;
