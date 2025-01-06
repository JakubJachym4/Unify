using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;

namespace Unify.Application.UniversityClasses.ClassOfferings.Commands;

public record AddClassOfferingCommand(string Name, Guid CourseId, DateOnly StartDate, DateOnly EndDate, Guid LecturerId, Guid? BoundGroupId, int MaxStudentsCount) : ICommand<Guid>;
public record UpdateClassOfferingCommand(Guid Id, string Name, DateOnly StartDate, DateOnly EndDate, int MaxStudentsCount) : ICommand;
public record DeleteClassOfferingCommand(Guid Id) : ICommand;
public record ListClassOfferingsQuery() : IQuery<List<Domain.UniversityClasses.ClassOffering>>;

public record EnrollStudentCommand(Guid ClassOfferingId) : ICommand<Guid>;