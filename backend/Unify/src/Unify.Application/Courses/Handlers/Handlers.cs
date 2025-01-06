using MediatR;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Courses.Commands;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Application.Courses.Handlers;

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

        course.Update(new Name(request.Name), new Description(request.Description), request.SpecialiationId);
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

internal sealed class ListCoursesQueryHandler : IRequestHandler<ListCoursesQuery, Result<List<Course>>>
{
    private readonly ICourseRepository _repository;

    public ListCoursesQueryHandler(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<Course>>> Handle(ListCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(courses.ToList());
    }
}