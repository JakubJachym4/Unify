using Unify.Application.Abstractions.Authentication;
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
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;
using Unify.Domain.Users;

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

public sealed class GradeHomeworkSubmissionCommandHandler : ICommandHandler<GradeHomeworkSubmissionCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IHomeworkSubmissionRepository _homeworkSubmissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHomeworkAssignmentRepository _homeworkAssignmentRepository;
    private readonly IUserContext _userContext;
    private readonly IGradeRepository _gradeRepository;
    private readonly IMarkRepository _markRepository;
    private readonly IClassOfferingRepository _classOfferingRepository;
    private readonly IClassEnrollmentRepository _classEnrollmentRepository;


    public GradeHomeworkSubmissionCommandHandler(IUserRepository userRepository, IHomeworkSubmissionRepository homeworkSubmissionRepository, IUnitOfWork unitOfWork, IHomeworkAssignmentRepository homeworkAssignmentRepository, IUserContext userContext, IGradeRepository gradeRepository, IMarkRepository markRepository, IClassOfferingRepository classOfferingRepository, IClassEnrollmentRepository classEnrollmentRepository)
    {
        _userRepository = userRepository;
        _homeworkSubmissionRepository = homeworkSubmissionRepository;
        _unitOfWork = unitOfWork;
        _homeworkAssignmentRepository = homeworkAssignmentRepository;
        _userContext = userContext;
        _gradeRepository = gradeRepository;
        _markRepository = markRepository;
        _classOfferingRepository = classOfferingRepository;
        _classEnrollmentRepository = classEnrollmentRepository;
    }

    public async Task<Result> Handle(GradeHomeworkSubmissionCommand request, CancellationToken cancellationToken)
    {
        var lecturer = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);
        if (lecturer == null)
        {
            return Result.Failure(UserErrors.NotFound(_userContext.UserId));
        }

        var homeworkAssignment = await _homeworkAssignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        if (homeworkAssignment == null)
        {
            return Result.Failure(HomeworkAssigmentErrors.NotFound);
        }

        var classOffering = await _classOfferingRepository.GetByIdAsync(homeworkAssignment.ClassOfferingId, cancellationToken);
        if (classOffering == null)
        {
            return Result.Failure(ClassOfferingErrors.NotFound);
        }

        if (classOffering.LecturerId != lecturer.Id)
        {
            return Result.Failure(UserErrors.LecturerNotAssigned);
        }

        var homeworkSubmission = await _homeworkSubmissionRepository.GetByIdAsync(request.SubmissionId, cancellationToken);
        if (homeworkSubmission == null)
        {
            return Result.Failure(HomeworkSubmissionErrors.NotFound);
        }

        var enrollment = await _classEnrollmentRepository.GetByClassOfferingAndStudentAsync(classOffering.Id, homeworkSubmission.StudentId, cancellationToken);
        if (enrollment == null)
        {
            return Result.Failure("Enrollment.NotFound", "The student is not enrolled in the class offering");
        }

        var grade = await _gradeRepository.GetByIdAsync(enrollment.GradeId, cancellationToken);

        if (grade == null)
        {
            return Result.Failure(GradeErrors.NotFound);
        }

        var mark = Mark.CreateForSubmission(grade, homeworkSubmission, request.Score, request.MaxScore,
            new Description(request.Criteria ?? ""));

        if (request.Feedback != null)
        {
            homeworkSubmission.ProvideFeedback(new TextContent(request.Feedback));
        }

        _markRepository.Add(mark);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public static class GradeErrors
{
    public static Error NotFound => Error.Create("Grade.NotFound", "The grade with the specified identifier was not found");
}