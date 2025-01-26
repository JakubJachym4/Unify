using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Lectures.CommandsAndQueries;

public record CreateLectureCommand(Guid CourseId, string Title, DateTime ScheduledDate, TimeSpan Duration, Guid LecturerId, Guid LocationId) : ICommand<Guid>;

public record UpdateLectureCommand(Guid Id, string Title, DateTime ScheduledDate, TimeSpan Duration, Guid LecturerId, Guid LocationId) : ICommand;

public record DeleteLectureCommand(Guid Id) : ICommand;

public record GetLectureQuery(Guid Id) : IQuery<LectureResponse>;

public record ListLecturesQuery : IQuery<List<LectureResponse>>;

public record ListLecturesByCourseQuery(Guid CourseId) : IQuery<List<LectureResponse>>;

public record CreateIntervalLecturesCommand(
    Guid CourseId,
    string Title,
    DateTime StartDate,
    DateTime EndDate,
    int WeekInterval,
    TimeSpan Duration,
    Guid LecturerId,
    Guid LocationId) : ICommand;