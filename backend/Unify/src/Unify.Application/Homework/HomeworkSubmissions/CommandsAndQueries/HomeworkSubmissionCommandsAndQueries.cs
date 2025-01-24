using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Homework.HomeworkSubmissions.CommandsAndQueries;


public record CreateHomeworkSubmissionCommand(Guid HomeworkAssignmentId, List<IFormFile>? Attachments) : ICommand<Guid>;

public record UpdateHomeworkSubmissionCommand(Guid Id, List<IFormFile>? Attachments) : ICommand;

public record DeleteHomeworkSubmissionCommand(Guid Id) : ICommand;