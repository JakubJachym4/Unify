using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.ClassOfferingSessions.CommandsAndQueries;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityClasses.Abstractions;
using Unify.Domain.UniversityCore.Abstractions;
using Unify.Domain.Users;
using Unify.Domain.UniversityCore.Errors;

namespace Unify.Application.ClassOfferingSessions.Handlers;

public sealed class CreateClassOfferingSessionCommandHandler : ICommandHandler<CreateClassOfferingSessionCommand, Guid>
{
    private readonly IClassOfferingRepository _classOfferingRepository;
    private readonly IClassOfferingSessionRepository _classOfferingSessionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateClassOfferingSessionCommandHandler(IClassOfferingRepository classOfferingRepository, IUserRepository userRepository, ILocationRepository locationRepository, IUnitOfWork unitOfWork, IClassOfferingSessionRepository classOfferingSessionRepository)
    {
        _classOfferingRepository = classOfferingRepository;
        _userRepository = userRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
        _classOfferingSessionRepository = classOfferingSessionRepository;
    }

    public async Task<Result<Guid>> Handle(CreateClassOfferingSessionCommand request, CancellationToken cancellationToken)
    {
        var classOffering = await _classOfferingRepository.GetByIdAsync(request.ClassOfferingId, cancellationToken);
        if (classOffering is null)
        {
            return Result.Failure<Guid>("ClassOffering.NotFound", "Class offering not found.");
        }

        var lecturer = await _userRepository.GetByIdAsync(request.LecturerId, cancellationToken);
        if (lecturer is null)
        {
            return Result.Failure<Guid>("Lecturer.NotFound", "Lecturer not found.");
        }

        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken);
        if (location is null)
        {
            return Result.Failure<Guid>("Location.NotFound", "Location not found.");
        }

        var session = new ClassOfferingSession(classOffering, new Title(request.Title), request.ScheduledDate, request.Duration, lecturer, location);

        _classOfferingSessionRepository.Add(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(session.Id);
    }
}

public sealed class UpdateClassOfferingSessionCommandHandler : ICommandHandler<UpdateClassOfferingSessionCommand>
{
    private readonly IClassOfferingSessionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateClassOfferingSessionCommandHandler(IClassOfferingSessionRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateClassOfferingSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (session is null)
        {
            return Result.Failure("ClassOfferingSession.NotFound", "Class offering session not found.");
        }

        session.Update(new Title(request.Title), request.ScheduledDate, request.Duration, request.LecturerId, request.LocationId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class DeleteClassOfferingSessionCommandHandler : ICommandHandler<DeleteClassOfferingSessionCommand>
{
    private readonly IClassOfferingSessionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteClassOfferingSessionCommandHandler(IClassOfferingSessionRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteClassOfferingSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (session is null)
        {
            return Result.Failure("ClassOfferingSession.NotFound", "Class offering session not found.");
        }

        _repository.Delete(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class GetClassOfferingSessionQueryHandler : IQueryHandler<GetClassOfferingSessionQuery, ClassOfferingSessionResponse>
{
    private readonly IClassOfferingSessionRepository _repository;

    public GetClassOfferingSessionQueryHandler(IClassOfferingSessionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ClassOfferingSessionResponse>> Handle(GetClassOfferingSessionQuery request, CancellationToken cancellationToken)
    {
        var session = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (session is null)
        {
            return Result.Failure<ClassOfferingSessionResponse>("ClassOfferingSession.NotFound", "Class offering session not found.");
        }

        return Result.Success(ClassOfferingSessionResponse.CreateFrom(session));
    }
}

public sealed class ListClassOfferingSessionsQueryHandler : IQueryHandler<ListClassOfferingSessionsQuery, List<ClassOfferingSessionResponse>>
{
    private readonly IClassOfferingSessionRepository _repository;

    public ListClassOfferingSessionsQueryHandler(IClassOfferingSessionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<ClassOfferingSessionResponse>>> Handle(ListClassOfferingSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(sessions.Select(ClassOfferingSessionResponse.CreateFrom).ToList());
    }
}

internal sealed class GetSessionByClassOfferingQueryHandler : IQueryHandler<GetSessionByClassOfferingQuery, List<ClassOfferingSessionResponse>>
{
    private readonly IClassOfferingSessionRepository _classOfferingSessionRepository;
    private readonly IClassOfferingRepository _classOfferingRepository;

    public GetSessionByClassOfferingQueryHandler(IClassOfferingSessionRepository classOfferingSessionRepository, IClassOfferingRepository classOfferingRepository)
    {
        _classOfferingSessionRepository = classOfferingSessionRepository;
        _classOfferingRepository = classOfferingRepository;
    }

    public async Task<Result<List<ClassOfferingSessionResponse>>> Handle(GetSessionByClassOfferingQuery request, CancellationToken cancellationToken)
    {
        var classOffering = await _classOfferingRepository.GetByIdAsync(request.ClassOfferingId, cancellationToken);
        if (classOffering is null)
        {
            return Result.Failure<List<ClassOfferingSessionResponse>>(ClassOfferingErrors.NotFound);
        }

        var sessions = await _classOfferingSessionRepository.GetByClassOfferingIdAsync(classOffering.Id, cancellationToken);
        return sessions.Select(ClassOfferingSessionResponse.CreateFrom).ToList();
    }
}

public sealed class GetSessionByStudentQueryHandler : IQueryHandler<GetSessionByStudentQuery, List<ClassOfferingSessionResponse>>
{
    private readonly IClassOfferingSessionRepository _classOfferingSessionRepository;
    private readonly IClassEnrollmentRepository _classEnrollmentRepository;
    private readonly IUserRepository _userRepository;

    public GetSessionByStudentQueryHandler(IClassOfferingSessionRepository classOfferingSessionRepository, IClassEnrollmentRepository classEnrollmentRepository, IUserRepository userRepository)
    {
        _classOfferingSessionRepository = classOfferingSessionRepository;
        _classEnrollmentRepository = classEnrollmentRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<ClassOfferingSessionResponse>>> Handle(GetSessionByStudentQuery request, CancellationToken cancellationToken)
    {
        var student = await _userRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student is null)
        {
            return Result.Failure<List<ClassOfferingSessionResponse>>(UserErrors.NotFound(request.StudentId));
        }
        if(student.Roles.All(role => role.Id != Role.Student.Id))
        {
            return Result.Failure<List<ClassOfferingSessionResponse>>("User.NotStudent", "User is not a student.");
        }

        var enrollments = await _classEnrollmentRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);
        var studentSessions = new List<ClassOfferingSession>();
        foreach (var enrollment in enrollments)
        {
            var enrollmentSessions = await _classOfferingSessionRepository.GetByClassOfferingIdAsync(enrollment.ClassOfferingId, cancellationToken);
            if (enrollmentSessions.Any())
            {
                studentSessions.AddRange(enrollmentSessions);
            }
        }

        return studentSessions.Select(ClassOfferingSessionResponse.CreateFrom).ToList();
    }
}

public sealed class GetSessionByLecturerQueryHandler : IQueryHandler<GetSessionByLecturerQuery, List<ClassOfferingSessionResponse>>
{
    private readonly IClassOfferingSessionRepository _classOfferingSessionRepository;
    private readonly IUserRepository _userRepository;

    public GetSessionByLecturerQueryHandler(IClassOfferingSessionRepository classOfferingSessionRepository, IUserRepository userRepository)
    {
        _classOfferingSessionRepository = classOfferingSessionRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<ClassOfferingSessionResponse>>> Handle(GetSessionByLecturerQuery request, CancellationToken cancellationToken)
    {
        var lecturer = await _userRepository.GetByIdAsync(request.LecturerId, cancellationToken);
        if (lecturer is null)
        {
            return Result.Failure<List<ClassOfferingSessionResponse>>(UserErrors.NotFound(request.LecturerId));
        }
        if(lecturer.Roles.All(role => role.Id != Role.Lecturer.Id))
        {
            return Result.Failure<List<ClassOfferingSessionResponse>>("User.NotLecturer", "User is not a lecturer.");
        }

        var sessions = await _classOfferingSessionRepository.GetByLecturerIdAsync(request.LecturerId, cancellationToken);
        return sessions.Select(ClassOfferingSessionResponse.CreateFrom).ToList();
    }
}

public sealed class CreateIntervalSessionsCommandHandler : ICommandHandler<CreateIntervalSessionsCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IClassOfferingRepository _classOfferingRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClassOfferingSessionRepository _sessionRepository;
    private readonly IUserContext _userContext;
    private readonly ICourseRepository _courseRepository;

    public CreateIntervalSessionsCommandHandler(IUserRepository userRepository, IClassOfferingRepository classOfferingRepository, ILocationRepository locationRepository, IUnitOfWork unitOfWork, IClassOfferingSessionRepository sessionRepository, IUserContext userContext, ICourseRepository courseRepository)
    {
        _userRepository = userRepository;
        _classOfferingRepository = classOfferingRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
        _sessionRepository = sessionRepository;
        _userContext = userContext;
        _courseRepository = courseRepository;
    }

    public async Task<Result> Handle(CreateIntervalSessionsCommand request, CancellationToken cancellationToken)
    {
        var lecturer = await _userRepository.GetByIdAsync(request.LecturerId, cancellationToken);
        if (lecturer == null)
        {
            return Result.Failure(UserErrors.NotFound(request.LecturerId));
        }
        var classOffering = await _classOfferingRepository.GetByIdAsync(request.ClassOfferingId, cancellationToken);
        if (classOffering == null)
        {
            return Result.Failure(ClassOfferingErrors.NotFound);
        }
        var course = await _courseRepository.GetByIdAsync(classOffering.CourseId, cancellationToken);
        if (course == null)
        {
            return Result.Failure("Course.NotFound", "Course not found.");
        }
        if (course.LecturerId != _userContext.UserId)
        {
            return Result.Failure("Course.NotLecturer", "User is not the lecturer of the course.");
        }

        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken);
        if (location == null)
        {
            return Result.Failure("Location.NotFound", "Location not found.");
        }

        var startDate = request.StartDate;
        var sessions = new List<ClassOfferingSession>();
        while (startDate < request.EndDate)
        {
            var session = new ClassOfferingSession(classOffering, new Title(request.Title), startDate, request.Duration, lecturer, location);
            sessions.Add(session);
            startDate = startDate.AddDays(request.WeekInterval * 7);
        }

        foreach (var session in sessions)
        {
            _sessionRepository.Add(session);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}