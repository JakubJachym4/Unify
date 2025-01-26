using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Homework.HomeworkAssignments.CommandsAndQueries;


public record CreateHomeworkAssignmentCommand(Guid ClassOfferingId, string Title, string Description, DateTime DueDate, List<IFormFile>? Attachments) : ICommand<Guid>;

public record UpdateHomeworkAssignmentCommand(Guid Id, string Title, string Description, DateTime DueDate, List<IFormFile>? Attachments) : ICommand;

public record DeleteHomeworkAssignmentCommand(Guid Id) : ICommand;

public record GradeHomeworkSubmissionCommand(Guid AssignmentId, Guid SubmissionId, Decimal Score, Decimal MaxScore, string? Criteria, string? Feedback) : ICommand;