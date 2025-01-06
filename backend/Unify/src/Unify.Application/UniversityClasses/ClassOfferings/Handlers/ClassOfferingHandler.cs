using MediatR;
using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Clock;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.UniversityClasses.ClassOfferings.Commands;
using Unify.Domain.Abstractions;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityClasses.Abstractions;
using Unify.Domain.UniversityCore.Abstractions;
using Unify.Domain.Users;

namespace Unify.Application.UniversityClasses.ClassOfferings.Handlers;

internal sealed class AddClassOfferingCommandHandler : ICommandHandler<AddClassOfferingCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClassOfferingRepository _repository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public AddClassOfferingCommandHandler(IUnitOfWork unitOfWork, IClassOfferingRepository repository, ICourseRepository courseRepository, IUserRepository userRepository, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<Guid>> Handle(AddClassOfferingCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            return Result.Failure<Guid>("Course.NotFound", "Course not found.");
        }

        var lecturer = await _userRepository.GetByIdAsync(request.LecturerId, cancellationToken);
        if (lecturer is null || lecturer.Roles.Any(role => role.Id == Role.Lecturer.Id) == false)
        {
            return Result.Failure<Guid>("Lecturer.NotFound", "Lecturer not found.");
        }

        var classOffering = ClassOffering.Create(new Name(request.Name), course, request.StartDate, request.EndDate, lecturer, null, request.MaxStudentsCount);
        _repository.Add(classOffering);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(classOffering.Id);
    }
}

internal sealed class UpdateClassOfferingCommandHandler : ICommandHandler<UpdateClassOfferingCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClassOfferingRepository _repository;

    public UpdateClassOfferingCommandHandler(IUnitOfWork unitOfWork, IClassOfferingRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(UpdateClassOfferingCommand request, CancellationToken cancellationToken)
    {
        var classOffering = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (classOffering is null)
        {
            return Result.Failure("ClassOfferings.NotFound" ,"Class Offering not found.");
        }

        classOffering.Update(new Name(request.Name), request.StartDate, request.EndDate, request.MaxStudentsCount);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class DeleteClassOfferingCommandHandler : ICommandHandler<DeleteClassOfferingCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClassOfferingRepository _repository;

    public DeleteClassOfferingCommandHandler(IUnitOfWork unitOfWork, IClassOfferingRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(DeleteClassOfferingCommand request, CancellationToken cancellationToken)
    {
        var classOffering = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (classOffering is null)
        {
            return Result.Failure("ClassOfferings.NotFound" ,"ClassOffering not found.");
        }

        _repository.Delete(classOffering);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class ListClassOfferingsQueryHandler : IQueryHandler<ListClassOfferingsQuery, List<ClassOffering>>
{
    private readonly IClassOfferingRepository _repository;

    public ListClassOfferingsQueryHandler(IClassOfferingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<ClassOffering>>> Handle(ListClassOfferingsQuery request, CancellationToken cancellationToken)
    {
        var classOfferings = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(classOfferings.ToList());
    }
}

internal sealed class EnrollClassOfferingHandler : ICommandHandler<EnrollStudentCommand, Guid>
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IClassOfferingRepository _classOfferingRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public EnrollClassOfferingHandler(IUnitOfWork unitOfWork, IClassOfferingRepository classOfferingRepository, ICourseRepository courseRepository, IUserRepository userRepository, IUserContext userContext, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _classOfferingRepository = classOfferingRepository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
        _userContext = userContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);
        if (user == null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(_userContext.UserId));
        }
        var classOffering = await _classOfferingRepository.GetByIdAsync(request.ClassOfferingId, cancellationToken);
        if (classOffering == null)
        {
            return Result.Failure<Guid>("ClassOfferings.NotFound" ,"Class Offering not found.");
        }

        var result = classOffering.Enroll(user, _dateTimeProvider.UtcNow, null);
        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(classOffering.Id);
    }
}