using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Clock;
using Unify.Application.Abstractions.Files;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Homework.HomeworkSubmissions.CommandsAndQueries;
using Unify.Domain.Abstractions;
using Unify.Domain.OnlineResources;
using Unify.Domain.OnlineResources.Abstraction;
using Unify.Domain.OnlineResources.Errors;
using Unify.Domain.Users;

namespace Unify.Application.Homework.HomeworkSubmissions.Handlers;

public sealed class CreateHomeworkSubmissionCommandHandler : ICommandHandler<CreateHomeworkSubmissionCommand, Guid>
{
    private readonly IHomeworkAssignmentRepository _homeworkAssignmentRepository;
    private readonly IHomeworkSubmissionRepository _homeworkSubmissionRepository;
    private readonly IUserContext _userContext;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileConversionService _fileConversionService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateHomeworkSubmissionCommandHandler(IHomeworkAssignmentRepository homeworkAssignmentRepository, IHomeworkSubmissionRepository homeworkSubmissionRepository, IUnitOfWork unitOfWork, IFileConversionService fileConversionService, IUserContext userContext, IUserRepository userRepository, IDateTimeProvider dateTimeProvider)
    {
        _homeworkAssignmentRepository = homeworkAssignmentRepository;
        _homeworkSubmissionRepository = homeworkSubmissionRepository;
        _unitOfWork = unitOfWork;
        _fileConversionService = fileConversionService;
        _userContext = userContext;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> Handle(CreateHomeworkSubmissionCommand request, CancellationToken cancellationToken)
    {
        var homeworkAssignment = await _homeworkAssignmentRepository.GetByIdAsync(request.HomeworkAssignmentId, cancellationToken);
        if (homeworkAssignment is null)
        {
            return Result.Failure<Guid>(HomeworkSubmissionErrors.NotFound);
        }

        var user = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);

        if(user == null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(_userContext.UserId));
        }

        var homeworkSubmission = HomeworkSubmission.Submit(homeworkAssignment, user, _dateTimeProvider.UtcNow);

        if (request.Attachments != null)
        {
            var attachments = await _fileConversionService.ConvertToAttachments(request.Attachments);

            foreach (var attachment in attachments)
            {
                if (attachment.IsFailure)
                {
                    return Result.Failure<Guid>(attachment.Error);
                }
                homeworkSubmission.AddAttachments(new[] { attachment.Value });
            }
        }

        _homeworkSubmissionRepository.Add(homeworkSubmission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(homeworkSubmission.Id);
    }
}

public sealed class UpdateHomeworkSubmissionCommandHandler : ICommandHandler<UpdateHomeworkSubmissionCommand>
{
    private readonly IHomeworkSubmissionRepository _homeworkSubmissionRepository;
    private readonly IFileConversionService _fileConversionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateHomeworkSubmissionCommandHandler(IHomeworkSubmissionRepository homeworkSubmissionRepository, IFileConversionService fileConversionService, IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _homeworkSubmissionRepository = homeworkSubmissionRepository;
        _fileConversionService = fileConversionService;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(UpdateHomeworkSubmissionCommand request, CancellationToken cancellationToken)
    {
        var homeworkSubmission = await _homeworkSubmissionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (homeworkSubmission is null)
        {
            return Result.Failure(HomeworkSubmissionErrors.NotFound);
        }

        homeworkSubmission.Update(_dateTimeProvider.UtcNow);

        if (request.Attachments != null)
        {
            var attachments = await _fileConversionService.ConvertToAttachments(request.Attachments);
            homeworkSubmission.ClearFiles();

            foreach (var attachment in attachments)
            {
                if (attachment.IsFailure)
                {
                    return Result.Failure(attachment.Error);
                }
                homeworkSubmission.AddAttachments(new[] { attachment.Value });
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class DeleteHomeworkSubmissionCommandHandler : ICommandHandler<DeleteHomeworkSubmissionCommand>
{
    private readonly IHomeworkSubmissionRepository _homeworkSubmissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHomeworkSubmissionCommandHandler(IHomeworkSubmissionRepository homeworkSubmissionRepository, IUnitOfWork unitOfWork)
    {
        _homeworkSubmissionRepository = homeworkSubmissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteHomeworkSubmissionCommand request, CancellationToken cancellationToken)
    {
        var homeworkSubmission = await _homeworkSubmissionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (homeworkSubmission is null)
        {
            return Result.Failure(HomeworkSubmissionErrors.NotFound);
        }

        _homeworkSubmissionRepository.Delete(homeworkSubmission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}