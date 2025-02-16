namespace Unify.Application.ClassEnrollment.CommandsAndQueries;

using Unify.Application.Abstractions.Messaging;


public record EnrollStudentCommand(Guid ClassOfferingId, Guid StudentId) : ICommand;

public record CancelEnrollmentStudentCommand(Guid ClassOfferingId, Guid StudentId) : ICommand;

public record GetEnrollmentsForClassOfferingQuery(Guid ClassOfferingId) : IQuery<List<ClassEnrollmentResponseWithGrade>>;

public record GetEnrollmentsForStudentQuery(Guid StudentId) : IQuery<List<ClassEnrollmentResponseWithGrade>>;
