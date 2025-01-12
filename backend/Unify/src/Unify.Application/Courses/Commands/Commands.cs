using Unify.Application.Abstractions.Messaging;
using Unify.Application.Courses.Handlers;
using Unify.Domain.UniversityCore;

namespace Unify.Application.Courses.Commands;

public record AddCourseCommand(string Name, string Description, Guid SpecializationId) : ICommand<Guid>;
public record UpdateCourseCommand(Guid Id, string Name, string Description, Guid SpecialiationId) : ICommand;
public record DeleteCourseCommand(Guid Id) : ICommand;
public record ListCoursesQuery() : IQuery<List<CourseResult>>;