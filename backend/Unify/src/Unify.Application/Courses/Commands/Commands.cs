using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Courses.CourseHandlers;
using Unify.Domain.UniversityCore;

namespace Unify.Application.Courses.Commands;

public record AddCourseCommand(string Name, string Description, Guid SpecializationId) : ICommand<Guid>;
public record UpdateCourseCommand(Guid Id, string Name, string Description) : ICommand;
public record DeleteCourseCommand(Guid Id) : ICommand;
public record ListCoursesQuery() : IQuery<List<CourseResponse>>;
public record ListCoursesBySpecializationQuery(Guid Id) : IQuery<List<CourseResponse>>;

public record AssignLecturerCommand(Guid Id, Guid LecturerId) : ICommand;

public record GetCoursesByLecturerQuery(Guid LecturerId) : IQuery<List<CourseResponse>>;

public record GetCourseQuery(Guid Id) : IQuery<CourseResponse>;

public record CreateCourseResourceCommand(Guid CourseId, string Title, string Description, ICollection<IFormFile>? Attachments) : ICommand<Guid>;

public record UpdateCourseResourceCommand(Guid Id, string Title, string Description, ICollection<IFormFile>? Attachments) : ICommand;

public record DeleteCourseResourceCommand(Guid Id) : ICommand;

public record GetCourseResourcesQuery(Guid Id) : IQuery<List<CourseResourceResponse>>;
public record GetCourseResourceQuery(Guid Id) : IQuery<CourseResourceResponse>;