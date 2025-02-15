using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Homework.HomeworkSubmissions.CommandsAndQueries;


public record CreateHomeworkSubmissionCommand(Guid HomeworkAssignmentId, List<IFormFile>? Attachments) : ICommand<Guid>;

public record UpdateHomeworkSubmissionCommand(Guid Id, List<IFormFile>? Attachments) : ICommand;

public record DeleteHomeworkSubmissionCommand(Guid Id) : ICommand;

public record GetHomeworkSubmissionQuery(Guid Id) : IQuery<HomeworkSubmissionResponse>;

public record GetHomeworkSubmissionsByAssignmentQuery(Guid HomeworkAssignmentId) : IQuery<List<HomeworkSubmissionResponse>>;

public record GetHomeworkSubmissionsByStudentQuery(Guid StudentId) : IQuery<List<HomeworkSubmissionResponse>>;