using Unify.Application.Abstractions.Messaging;
using Unify.Application.Courses.CourseHandlers;
using Unify.Domain.Abstractions;

namespace Unify.Application.UniversityClasses.ClassOfferings.Commands;

public record AddClassOfferingCommand(string Name, Guid CourseId, DateOnly StartDate, DateOnly EndDate, Guid LecturerId, Guid StudentGroupId, int MaxStudentsCount) : ICommand<Guid>;
public record UpdateClassOfferingCommand(Guid Id, string Name, DateOnly StartDate, DateOnly EndDate, int MaxStudentsCount) : ICommand;
public record DeleteClassOfferingCommand(Guid Id) : ICommand;
public record ListClassOfferingsQuery() : IQuery<List<Domain.UniversityClasses.ClassOffering>>;


public record AssignLecturerCommand(Guid Id, Guid LecturerId) : ICommand;

public record GetClassOfferingsByLecturerQuery(Guid LecturerId) : IQuery<List<ClassOfferingResponse>>;

public record GetClassOfferingQuery(Guid Id) : IQuery<ClassOfferingResponse>;


public record GetClassOfferingsByStudentQuery(Guid StudentId) : IQuery<List<ClassOfferingResponse>>;