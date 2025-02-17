using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Clock;
using Unify.Application.Abstractions.Files;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Files;
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

        var criteria = request.Criteria != null ? new Description(request.Criteria) : null;
        var homeworkAssignment = new HomeworkAssignment(classOffering, new Title(request.Title), new Description(request.Description), criteria, request.DueDate);

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

        var criteria = request.Criteria != null ? new Description(request.Criteria) : null;
        homeworkAssignment.Update(new Title(request.Title), new Description(request.Description), criteria, request.DueDate);

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
    private readonly IDateTimeProvider _dateTimeProvider;


    public GradeHomeworkSubmissionCommandHandler(IUserRepository userRepository, IHomeworkSubmissionRepository homeworkSubmissionRepository, IUnitOfWork unitOfWork, IHomeworkAssignmentRepository homeworkAssignmentRepository, IUserContext userContext, IGradeRepository gradeRepository, IMarkRepository markRepository, IClassOfferingRepository classOfferingRepository, IClassEnrollmentRepository classEnrollmentRepository, IDateTimeProvider dateTimeProvider)
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
        _dateTimeProvider = dateTimeProvider;
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

        var mark = Mark.CreateForSubmission(new Title(request.Title), grade, homeworkSubmission, request.Score, request.MaxScore,
            _dateTimeProvider.UtcNow);

        if (request.Feedback != null)
        {
            homeworkSubmission.ProvideFeedback(new TextContent(request.Feedback));
        }

        _markRepository.Add(mark);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class GetHomeworkAssignmentQueryHandler : IQueryHandler<GetHomeworkAssignmentQuery, HomeworkAssigmentResponse>
{
    private readonly IHomeworkAssignmentRepository _homeworkAssignmentRepository;

    public GetHomeworkAssignmentQueryHandler(IHomeworkAssignmentRepository homeworkAssignmentRepository)
    {
        _homeworkAssignmentRepository = homeworkAssignmentRepository;
    }

    public async Task<Result<HomeworkAssigmentResponse>> Handle(GetHomeworkAssignmentQuery request, CancellationToken cancellationToken)
    {
        var homeworkAssignment = await _homeworkAssignmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (homeworkAssignment is null)
        {
            return Result.Failure<HomeworkAssigmentResponse>(HomeworkAssigmentErrors.NotFound);
        }

        return Result.Success(new HomeworkAssigmentResponse(homeworkAssignment.Id, homeworkAssignment.ClassOfferingId, homeworkAssignment.Title.Value,
            homeworkAssignment.Description.Value, homeworkAssignment.Criteria?.Value, homeworkAssignment.DueDate, homeworkAssignment.Locked,
            FileResponse.FromManyAttachments(homeworkAssignment?.Attachments.ToList() ?? new List<Attachment>())));
    }
}

public sealed class GetHomeworkAssignmentByClassOfferingQueryHandler : IQueryHandler<GetHomeworkAssignmentsByClassOfferingQuery, List<HomeworkAssigmentResponse>>
{
    private readonly IHomeworkAssignmentRepository _homeworkAssignmentRepository;
    private readonly IClassOfferingRepository _classOfferingRepository;

    public GetHomeworkAssignmentByClassOfferingQueryHandler(IHomeworkAssignmentRepository homeworkAssignmentRepository, IClassOfferingRepository classOfferingRepository)
    {
        _homeworkAssignmentRepository = homeworkAssignmentRepository;
        _classOfferingRepository = classOfferingRepository;
    }

    public async Task<Result<List<HomeworkAssigmentResponse>>> Handle(GetHomeworkAssignmentsByClassOfferingQuery request, CancellationToken cancellationToken)
    {

        var classOffering = await _classOfferingRepository.GetByIdAsync(request.ClassOfferingId, cancellationToken);

        if (classOffering == null)
        {
            return Result.Failure<List<HomeworkAssigmentResponse>>(ClassOfferingErrors.NotFound);
        }

        var homeworkAssignments = await _homeworkAssignmentRepository.GetByClassOffering(classOffering, cancellationToken);
        if (homeworkAssignments is null || !homeworkAssignments.Any())
        {
            return new List<HomeworkAssigmentResponse>();
        }

        var response = homeworkAssignments.Select(ha => new HomeworkAssigmentResponse(
            ha.Id, ha.ClassOfferingId, ha.Title.Value, ha.Description.Value, ha.Criteria?.Value, ha.DueDate, ha.Locked,
            FileResponse.FromManyAttachments(ha.Attachments.ToList())
        )).ToList();

        return Result.Success(response);
    }
}

public sealed class GetHomeworkAssignmentsByStudentQueryHandler : IQueryHandler<GetHomeworkAssignmentsByStudentQuery, List<HomeworkAssigmentResponse>>
{

    private readonly IUserRepository _userRepository;
    private readonly IHomeworkAssignmentRepository _homeworkAssignmentRepository;
    private readonly IClassEnrollmentRepository _classEnrollmentRepository;
    private readonly IClassOfferingRepository _classOfferingRepository;

    public GetHomeworkAssignmentsByStudentQueryHandler(IUserRepository userRepository, IHomeworkAssignmentRepository homeworkAssignmentRepository, IClassEnrollmentRepository classEnrollmentRepository, IClassOfferingRepository classOfferingRepository)
    {
        _userRepository = userRepository;
        _homeworkAssignmentRepository = homeworkAssignmentRepository;
        _classEnrollmentRepository = classEnrollmentRepository;
        _classOfferingRepository = classOfferingRepository;
    }

    public async Task<Result<List<HomeworkAssigmentResponse>>> Handle(GetHomeworkAssignmentsByStudentQuery request, CancellationToken cancellationToken)
    {
        var student = await _userRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student is null)
        {
            return Result.Failure<List<HomeworkAssigmentResponse>>(UserErrors.NotFound(request.StudentId));
        }

        var enrollments = await _classEnrollmentRepository.GetByStudentIdAsync(student.Id, cancellationToken);
        var offeringsTasks = enrollments.Select(e => _classOfferingRepository.GetByIdAsync(e.ClassOfferingId, cancellationToken));

        var offerings = await Task.WhenAll(offeringsTasks);

        if (offerings.Length == 0)
        {
            return new List<HomeworkAssigmentResponse>();
        }

        var assignmentsTasks = offerings.Select(o =>
        {
            if (o != null) return _homeworkAssignmentRepository.GetByClassOffering(o, cancellationToken);
            return null;
        }).Where(task => task != null);

        var assignments = await Task.WhenAll(assignmentsTasks!);

        return assignments.SelectMany(x => x).Select(ha => new HomeworkAssigmentResponse(
            ha.Id, ha.ClassOfferingId, ha.Title.Value, ha.Description.Value, ha.Criteria?.Value, ha.DueDate, ha.Locked,
            FileResponse.FromManyAttachments(ha.Attachments.ToList())
        )).ToList();
    }
}

public sealed class LockHomeworkAssignmentCommandHandler : ICommandHandler<LockHomeworkAssignmentCommand>
{
    private readonly IHomeworkAssignmentRepository _homeworkAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LockHomeworkAssignmentCommandHandler(IHomeworkAssignmentRepository homeworkAssignmentRepository, IUnitOfWork unitOfWork)
    {
        _homeworkAssignmentRepository = homeworkAssignmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(LockHomeworkAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _homeworkAssignmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (assignment == null)
        {
            return Result.Failure(HomeworkAssigmentErrors.NotFound);
        }

        if (request.Locked)
        {
            assignment.LockSubmission();
        }
        else
        {
            assignment.UnlockSubmission();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public static class GradeErrors
{
    public static Error NotFound => Error.Create("Grade.NotFound", "The grade with the specified identifier was not found");
}