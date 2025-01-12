using MediatR;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Locations.Commands;
using Unify.Domain.Abstractions;
using Unify.Domain.UniversityCore;
using Unify.Domain.UniversityCore.Abstractions;

namespace Unify.Application.Locations.Handlers;

internal sealed class AddLocationCommandHandler : ICommandHandler<AddLocationCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocationRepository _repository;
    private readonly IFacultyRepository _facultyRepository;

    public AddLocationCommandHandler(IUnitOfWork unitOfWork, ILocationRepository repository, IFacultyRepository facultyRepository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _facultyRepository = facultyRepository;
    }

    public async Task<Result<Guid>> Handle(AddLocationCommand request, CancellationToken cancellationToken)
    {
        var faculty = await _facultyRepository.GetByIdAsync(request.FacultyId, cancellationToken);
        if (faculty is null)
        {
            return Result.Failure<Guid>("Location.NotFound", "Location not found.");
        }

        var location = Location.Add(request.Building, request.Street, request.Floor, request.DoorNumber, faculty);
        _repository.Add(location);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(location.Id);
    }
}

internal sealed class AddOnlineLocationCommandHandler : ICommandHandler<AddOnlineLocationCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocationRepository _repository;

    public AddOnlineLocationCommandHandler(IUnitOfWork unitOfWork, ILocationRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle(AddOnlineLocationCommand request, CancellationToken cancellationToken)
    {
        var location = Location.AddOnline(request.MeetingUrl);
        _repository.Add(location);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(location.Id);
    }
}

internal sealed class UpdateLocationCommandHandler : ICommandHandler<UpdateLocationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocationRepository _repository;

    public UpdateLocationCommandHandler(IUnitOfWork unitOfWork, ILocationRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (location is null)
        {
            return Result.Failure("Location.NotFound", "Location not found.");
        }

        location.Update(request.Building, request.Street, request.Floor, request.DoorNumber);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class UpdateOnlineLocationCommandHandler : ICommandHandler<UpdateOnlineLocationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocationRepository _repository;

    public UpdateOnlineLocationCommandHandler(IUnitOfWork unitOfWork, ILocationRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(UpdateOnlineLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (location is null)
        {
            return Result.Failure("Location.NotFound", "Location not found.");
        }

        location.UpdateOnline(request.MeetingUrl);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class DeleteLocationCommandHandler : ICommandHandler<DeleteLocationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocationRepository _repository;

    public DeleteLocationCommandHandler(IUnitOfWork unitOfWork, ILocationRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (location is null)
        {
            return Result.Failure("Location.NotFound", "Location not found.");
        }

        _repository.Delete(location);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed class ListLocationsQueryHandler : IRequestHandler<ListLocationsQuery, Result<List<LocationResult>>>
{
    private readonly ILocationRepository _repository;

    public ListLocationsQueryHandler(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<LocationResult>>> Handle(ListLocationsQuery request, CancellationToken cancellationToken)
    {
        var locations = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(LocationResult.FromLocations(locations));
    }
}

public sealed record LocationResult
{
    public static List<LocationResult> FromLocations(IEnumerable<Location> locations)
    {
        return locations.Select(location => new LocationResult(location)).ToList();
    }

    public LocationResult(Location location)
    {
        Id = location.Id;
        Building = location.Building;
        Street = location.Street;
        Floor = location.Floor;
        DoorNumber = location.DoorNumber;
        FacultyId = location.FacultyId;
        Online = location.Online;
        MeetingUrl = location.MeetingUrl;
    }

    public Guid Id { get; private set; }
    public string? Building { get; private set; }
    public string? Street { get; private set; }
    public short? Floor { get; private set; }
    public string? DoorNumber { get; private set; }
    public Guid? FacultyId { get; private set; }

    public bool Online { get; private set; }
    public string? MeetingUrl { get; private set; }
}