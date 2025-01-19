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
    private readonly IUserRepository _userRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateClassOfferingSessionCommandHandler(IClassOfferingRepository classOfferingRepository, IUserRepository userRepository, ILocationRepository locationRepository, IUnitOfWork unitOfWork)
    {
        _classOfferingRepository = classOfferingRepository;
        _userRepository = userRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
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