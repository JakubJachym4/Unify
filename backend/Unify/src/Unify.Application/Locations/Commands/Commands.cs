using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Locations.Commands;

public record AddLocationCommand(string Building, string Street, short Floor, string DoorNumber, Guid FacultyId) : ICommand<Guid>;
public record AddOnlineLocationCommand(string MeetingUrl) : ICommand<Guid>;
public record UpdateLocationCommand(Guid Id, string Building, string Street, short Floor, string DoorNumber) : ICommand;
public record UpdateOnlineLocationCommand(Guid Id, string MeetingUrl) : ICommand;
public record DeleteLocationCommand(Guid Id) : ICommand;
public record ListLocationsQuery() : IQuery<List<Domain.UniversityCore.Location>>;