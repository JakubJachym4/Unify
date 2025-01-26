using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Lectures.CommandsAndQueries;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore.Abstractions;
using Unify.Domain.Users;
using Unify.Domain.UniversityCore.Errors;

namespace Unify.Application.Lectures.Handlers;

public sealed class CreateLectureCommandHandler : ICommandHandler<CreateLectureCommand, Guid>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILectureRepository _lectureRepository;

    public CreateLectureCommandHandler(ICourseRepository courseRepository, IUserRepository userRepository, ILocationRepository locationRepository, IUnitOfWork unitOfWork, ILectureRepository lectureRepository)
    {
        _courseRepository = courseRepository;
        _userRepository = userRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
        _lectureRepository = lectureRepository;
    }

    public async Task<Result<Guid>> Handle(CreateLectureCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            return Result.Failure<Guid>("Course.NotFound", "Course not found.");
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

        var lecture = new Lecture(course, new Title(request.Title), request.ScheduledDate, request.Duration, lecturer, location);
        _lectureRepository.Add(lecture);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(lecture.Id);
    }
}

public sealed class UpdateLectureCommandHandler : ICommandHandler<UpdateLectureCommand>
{
    private readonly ILectureRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLectureCommandHandler(ILectureRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateLectureCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (lecture is null)
        {
            return Result.Failure("Lecture.NotFound", "Lecture not found.");
        }

        lecture.Update(new Title(request.Title), request.ScheduledDate, request.Duration, request.LecturerId, request.LocationId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class DeleteLectureCommandHandler : ICommandHandler<DeleteLectureCommand>
{
    private readonly ILectureRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLectureCommandHandler(ILectureRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteLectureCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (lecture is null)
        {
            return Result.Failure("Lecture.NotFound", "Lecture not found.");
        }

        _repository.Delete(lecture);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class GetLectureQueryHandler : IQueryHandler<GetLectureQuery, LectureResponse>
{
    private readonly ILectureRepository _repository;

    public GetLectureQueryHandler(ILectureRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<LectureResponse>> Handle(GetLectureQuery request, CancellationToken cancellationToken)
    {
        var lecture = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (lecture is null)
        {
            return Result.Failure<LectureResponse>("Lecture.NotFound", "Lecture not found.");
        }

        return Result.Success(LectureResponse.CreateFrom(lecture));
    }
}

public sealed class ListLecturesQueryHandler : IQueryHandler<ListLecturesQuery, List<LectureResponse>>
{
    private readonly ILectureRepository _repository;

    public ListLecturesQueryHandler(ILectureRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<LectureResponse>>> Handle(ListLecturesQuery request, CancellationToken cancellationToken)
    {
        var lectures = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(lectures.Select(LectureResponse.CreateFrom).ToList());
    }
}

public sealed class ListLecturesByCourseQueryHandler : IQueryHandler<ListLecturesByCourseQuery, List<LectureResponse>>
{
    private readonly ILectureRepository _repository;

    public ListLecturesByCourseQueryHandler(ILectureRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<LectureResponse>>> Handle(ListLecturesByCourseQuery request, CancellationToken cancellationToken)
    {
        var lectures = await _repository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        return Result.Success(lectures.Select(LectureResponse.CreateFrom).ToList());
    }
}

public sealed class CreateIntervalLecturesCommandHandler : ICommandHandler<CreateIntervalLecturesCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILectureRepository _lectureRepository;
    private readonly IUserContext _userContext;
    private readonly ICourseRepository _courseRepository;

    public CreateIntervalLecturesCommandHandler(IUserRepository userRepository, ILocationRepository locationRepository, IUnitOfWork unitOfWork, ILectureRepository lectureRepository, IUserContext userContext, ICourseRepository courseRepository)
    {
        _userRepository = userRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
        _lectureRepository = lectureRepository;
        _userContext = userContext;
        _courseRepository = courseRepository;
    }

    public async Task<Result> Handle(CreateIntervalLecturesCommand request, CancellationToken cancellationToken)
    {
        var lecturer = await _userRepository.GetByIdAsync(request.LecturerId, cancellationToken);
        if (lecturer == null)
        {
            return Result.Failure(UserErrors.NotFound(request.LecturerId));
        }

        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
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
        var lectures = new List<Lecture>();
        while (startDate < request.EndDate)
        {
            var lecture = new Lecture(course, new Title(request.Title), startDate, request.Duration, lecturer, location);
            lectures.Add(lecture);
            startDate = startDate.AddDays(request.WeekInterval * 7);
        }

        foreach (var session in lectures)
        {
            _lectureRepository.Add(session);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}