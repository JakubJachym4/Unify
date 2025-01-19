using Unify.Application.Abstractions.Clock;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.ClassEnrollment.CommandsAndQueries;
using Unify.Domain.Abstractions;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityClasses.Abstractions;
using Unify.Domain.Users;

namespace Unify.Application.ClassEnrollment.Handlers;

public sealed class EnrollStudentCommandHandler : ICommandHandler<EnrollStudentCommand>
{
    private readonly IClassOfferingRepository _classOfferingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public EnrollStudentCommandHandler(IClassOfferingRepository classOfferingRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _classOfferingRepository = classOfferingRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        var classOffering = await _classOfferingRepository.GetByIdAsync(request.ClassOfferingId, cancellationToken);
        if (classOffering == null)
        {
            return Result.Failure(ClassOfferingErrors.NotFound);
        }

        var student = await _userRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student == null)
        {
            return Result.Failure(UserErrors.NotFound(request.StudentId));
        }

        var result = classOffering.Enroll(student, _dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class CancelEnrollmentStudentCommandHandler : ICommandHandler<CancelEnrollmentStudentCommand>
{
    private readonly IClassOfferingRepository _classOfferingRepository;
    private readonly IClassEnrollmentRepository _classEnrollmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelEnrollmentStudentCommandHandler(IClassOfferingRepository classOfferingRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IClassEnrollmentRepository classEnrollmentRepository)
    {
        _classOfferingRepository = classOfferingRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _classEnrollmentRepository = classEnrollmentRepository;
    }

    public async Task<Result> Handle(CancelEnrollmentStudentCommand request, CancellationToken cancellationToken)
    {
        var classOffering = await _classOfferingRepository.GetByIdAsync(request.ClassOfferingId, cancellationToken);
        if (classOffering == null)
        {
            return Result.Failure(ClassOfferingErrors.NotFound);
        }

        var student = await _userRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student == null)
        {
            return Result.Failure(UserErrors.NotFound(request.StudentId));
        }

        var result = classOffering.CancelEnrollment(student);
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }
        //TODO: Check if needed for delete of if it's handled by ef
        //_classEnrollmentRepository.Delete(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class GetEnrollmentsForClassOfferingQueryHandler : IQueryHandler<GetEnrollmentsForClassOfferingQuery, List<ClassEnrollmentResponse>>
{
    private readonly IClassEnrollmentRepository _classEnrollmentRepository;

    public GetEnrollmentsForClassOfferingQueryHandler(IClassEnrollmentRepository classEnrollmentRepository)
    {
        _classEnrollmentRepository = classEnrollmentRepository;
    }

    public async Task<Result<List<ClassEnrollmentResponse>>> Handle(GetEnrollmentsForClassOfferingQuery request, CancellationToken cancellationToken)
    {
        var enrollments = await _classEnrollmentRepository.GetByClassOfferingIdAsync(request.ClassOfferingId, cancellationToken);
        return Result.Success(enrollments.Select(ClassEnrollmentResponse.CreateFrom).ToList());
    }
}

public sealed class GetEnrollmentsForStudentQueryHandler : IQueryHandler<GetEnrollmentsForStudentQuery, List<ClassEnrollmentResponse>>
{
    private readonly IClassEnrollmentRepository _classEnrollmentRepository;

    public GetEnrollmentsForStudentQueryHandler(IClassEnrollmentRepository classEnrollmentRepository)
    {
        _classEnrollmentRepository = classEnrollmentRepository;
    }

    public async Task<Result<List<ClassEnrollmentResponse>>> Handle(GetEnrollmentsForStudentQuery request, CancellationToken cancellationToken)
    {
        var enrollments = await _classEnrollmentRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);
        return Result.Success(enrollments.Select(ClassEnrollmentResponse.CreateFrom).ToList());
    }
}