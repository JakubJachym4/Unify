using Unify.Application.Abstractions.Files;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Homework.HomeworkAssignments.CommandsAndQueries;
using Unify.Domain.Abstractions;
using Unify.Domain.OnlineResources;
using Unify.Domain.OnlineResources.Abstraction;
using Unify.Domain.OnlineResources.Errors;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityClasses.Abstractions;

namespace Unify.Application.Homework.HomeworkAssignments.Handlers;


public sealed class CreateHomeworkAssignmentCommandHandler : ICommandHandler<CreateHomeworkAssignmentCommand, Guid>
{
    private readonly IClassOfferingRepository _classOfferingRepository;
    private readonly IHomeworkAssignmentRepository _homeworkAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileConversionService _fileConversionService;

    public CreateHomeworkAssignmentCommandHandler(IClassOfferingRepository classOfferingRepository, IHomeworkAssignmentRepository homeworkAssignmentRepository, IUnitOfWork unitOfWork, IFileConversionService fileConversionService)
    {
        _classOfferingRepository = classOfferingRepository;
        _homeworkAssignmentRepository = homeworkAssignmentRepository;
        _unitOfWork = unitOfWork;
        _fileConversionService = fileConversionService;
    }

    public async Task<Result<Guid>> Handle(CreateHomeworkAssignmentCommand request, CancellationToken cancellationToken)
    {
        var classOffering = await _classOfferingRepository.GetByIdAsync(request.ClassOfferingId, cancellationToken);
        if (classOffering is null)
        {
            return Result.Failure<Guid>(ClassOfferingErrors.NotFound);
        }

        var homeworkAssignment = new HomeworkAssigment(classOffering, new Title(request.Title), new Description(request.Description), request.DueDate);

        if (request.Attachments != null)
        {
            var attachments = await _fileConversionService.ConvertToAttachments(request.Attachments);

            foreach (var attachment in attachments)
            {
                if (attachment.IsFailure)
                {
                    return Result.Failure<Guid>(attachment.Error);
                }
                homeworkAssignment.AttachFile(attachment.Value);
            }
        }

        _homeworkAssignmentRepository.Add(homeworkAssignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(homeworkAssignment.Id);
    }
}

public sealed class UpdateHomeworkAssignmentCommandHandler : ICommandHandler<UpdateHomeworkAssignmentCommand>
{
    private readonly IHomeworkAssignmentRepository _homeworkAssignmentRepository;
    private readonly IFileConversionService _fileConversionService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHomeworkAssignmentCommandHandler(IHomeworkAssignmentRepository homeworkAssignmentRepository, IFileConversionService fileConversionService, IUnitOfWork unitOfWork)
    {
        _homeworkAssignmentRepository = homeworkAssignmentRepository;
        _fileConversionService = fileConversionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateHomeworkAssignmentCommand request, CancellationToken cancellationToken)
    {
        var homeworkAssignment = await _homeworkAssignmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (homeworkAssignment is null)
        {
            return Result.Failure(HomeworkAssigmentErrors.NotFound);
        }

        homeworkAssignment.Update(new Title(request.Title), new Description(request.Description), request.DueDate);

        if (request.Attachments != null)
        {
            var attachments = await _fileConversionService.ConvertToAttachments(request.Attachments);
            homeworkAssignment.ClearFiles();

            foreach (var attachment in attachments)
            {
                if (attachment.IsFailure)
                {
                    return Result.Failure(attachment.Error);
                }
                homeworkAssignment.AttachFile(attachment.Value);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class DeleteHomeworkAssignmentCommandHandler : ICommandHandler<DeleteHomeworkAssignmentCommand>
{
    private readonly IHomeworkAssignmentRepository _homeworkAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHomeworkAssignmentCommandHandler(IHomeworkAssignmentRepository homeworkAssignmentRepository, IUnitOfWork unitOfWork)
    {
        _homeworkAssignmentRepository = homeworkAssignmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteHomeworkAssignmentCommand request, CancellationToken cancellationToken)
    {
        var homeworkAssignment = await _homeworkAssignmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (homeworkAssignment is null)
        {
            return Result.Failure(HomeworkAssigmentErrors.NotFound);
        }

        _homeworkAssignmentRepository.Delete(homeworkAssignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}