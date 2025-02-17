using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Homework.HomeworkAssignments.CommandsAndQueries;


public record CreateHomeworkAssignmentCommand(Guid ClassOfferingId, string Title, string Description, string? Criteria, DateTime DueDate, List<IFormFile>? Attachments) : ICommand<Guid>;

public record UpdateHomeworkAssignmentCommand(Guid Id, string Title, string Description, string? Criteria, DateTime DueDate, List<IFormFile>? Attachments) : ICommand;

public record DeleteHomeworkAssignmentCommand(Guid Id) : ICommand;

public record LockHomeworkAssignmentCommand(Guid Id, bool Locked) : ICommand;

public record GradeHomeworkSubmissionCommand(Guid AssignmentId, Guid SubmissionId, Decimal Score, Decimal MaxScore, string? Feedback) : ICommand;

public record GetHomeworkAssignmentQuery(Guid Id) : IQuery<HomeworkAssigmentResponse>;
public record GetHomeworkAssignmentsByClassOfferingQuery(Guid ClassOfferingId) : IQuery<List<HomeworkAssigmentResponse>>;

public record GetHomeworkAssignmentsByStudentQuery(Guid StudentId) : IQuery<List<HomeworkAssigmentResponse>>;
