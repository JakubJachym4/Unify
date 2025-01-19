using MediatR;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Courses.Commands;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;
using Unify.Domain.Users;

namespace Unify.Application.Courses.CourseHandlers;

internal sealed class AddCourseCommandHandler : ICommandHandler<AddCourseCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICourseRepository _repository;
    private readonly ISpecializationRepository _specializationRepository;

    public AddCourseCommandHandler(IUnitOfWork unitOfWork, ICourseRepository repository, ISpecializationRepository specializationRepository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _specializationRepository = specializationRepository;
    }

    public async Task<Result<Guid>> Handle(AddCourseCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.SpecializationId, cancellationToken);
        if (specialization is null)
        {
            return Result.Failure<Guid>("Course.NotFound", "Course not found.");
        }

        var course = Course.Create(new Name(request.Name), new Description(request.Description), specialization);
        _repository.Add(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(course.Id);
    }
}

internal sealed class UpdateCourseCommandHandler : ICommandHandler<UpdateCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICourseRepository _repository;

    public UpdateCourseCommandHandler(IUnitOfWork unitOfWork, ICourseRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (course is null)
        {
            return Result.Failure("Course.NotFound", "Course not found.");
        }

        course.Update(new Name(request.Name), new Description(request.Description));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class DeleteCourseCommandHandler : ICommandHandler<DeleteCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICourseRepository _repository;

    public DeleteCourseCommandHandler(IUnitOfWork unitOfWork, ICourseRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (course is null)
        {
            return Result.Failure("Course.NotFound", "Course not found.");
        }

        _repository.Delete(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class ListCoursesQueryHandler : IRequestHandler<ListCoursesQuery, Result<List<CourseResponse>>>
{
    private readonly ICourseRepository _repository;

    public ListCoursesQueryHandler(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<CourseResponse>>> Handle(ListCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(
            courses.Select(c =>
                new CourseResponse(c.Id, c.Name.Value, c.Description.Value, c.SpecializationId, c.LecturerId, ClassOfferingResponse.FromClassOfferingList(c.Classes.ToList()))
            ).ToList()
            );
    }
}

internal sealed class ListCoursesBySpecializationQueryHandler : IQueryHandler<ListCoursesBySpecializationQuery, List<CourseResponse>>
{
    private readonly ICourseRepository _repository;
    private readonly ISpecializationRepository _specializationRepository;

    public ListCoursesBySpecializationQueryHandler(ICourseRepository repository, ISpecializationRepository specializationRepository)
    {
        _repository = repository;
        _specializationRepository = specializationRepository;
    }

    public async Task<Result<List<CourseResponse>>> Handle(ListCoursesBySpecializationQuery request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (specialization == null)
        {
            return Result.Failure<List<CourseResponse>>("Specialization.NotFound", "Specialization not found.");
        }

        var courses = await _repository.GetAllBySpecializationAsync(specialization, cancellationToken);

        return courses.Select(CourseResponse.CreateFromCourse).ToList();
    }
}

public sealed class AssignLecturerCommandHandler : ICommandHandler<AssignLecturerCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignLecturerCommandHandler(IUserRepository userRepository, ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AssignLecturerCommand request, CancellationToken cancellationToken)
    {
        var lecturer = await _userRepository.GetByIdAsync(request.LecturerId, cancellationToken);
        if (lecturer == null)
        {
            return Result.Failure(UserErrors.NotFound(request.LecturerId));
        }

        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (course == null)
        {
            return Result.Failure("Course.NotFound", "Course not found.");
        }

        course.AssignLecturer(lecturer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class GetCourseQueryHandler : IQueryHandler<GetCourseQuery, CourseResponse>
{
    private readonly ICourseRepository _repository;

    public GetCourseQueryHandler(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CourseResponse>> Handle(GetCourseQuery request, CancellationToken cancellationToken)
    {
        var course = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (course == null)
        {
            return Result.Failure<CourseResponse>("Course.NotFound", "Course not found.");
        }

        return Result.Success(CourseResponse.CreateFromCourse(course));
    }
}

public sealed class GetCoursesByLecturerQueryHandler : IQueryHandler<GetCoursesByLecturerQuery, List<CourseResponse>>
{
    private readonly ICourseRepository _repository;
    private readonly IUserRepository _userRepository;

    public GetCoursesByLecturerQueryHandler(ICourseRepository repository, IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<CourseResponse>>> Handle(GetCoursesByLecturerQuery request, CancellationToken cancellationToken)
    {
        var lecturer = await _userRepository.GetByIdAsync(request.LecturerId, cancellationToken);

        if (lecturer == null)
        {
            return Result.Failure<List<CourseResponse>>(UserErrors.NotFound(request.LecturerId));
        }

        var courses = await _repository.GetByLecturerIdAsync(lecturer, cancellationToken);
        return Result.Success(courses.Select(CourseResponse.CreateFromCourse).ToList());
    }
}

public record CourseResponse(
    Guid Id,
    string Name,
    string Description,
    Guid SpecializationId,
    Guid? LecturerId,
    List<ClassOfferingResponse> ClassOfferingResponses)
{
    public static CourseResponse CreateFromCourse(Course course)
    {
        return new CourseResponse(course.Id, course.Name.Value, course.Description.Value, course.SpecializationId, course.LecturerId, ClassOfferingResponse.FromClassOfferingList(course.Classes.ToList()));
    }
};

