using Unify.Domain.Abstractions;

namespace Unify.Domain.UniversityCore;

public sealed class Location : Entity
{
    public string? Building { get; private set; }
    public string? Street { get; private set; }
    public short? Floor { get; private set; }
    public string? DoorNumber { get; private set; }
    public Guid? FacultyId { get; private set; }
    
    public bool Online { get; private set; }
    public string? MeetingUrl { get; private set; }

    private Location(string meetingUrl) : base(Guid.NewGuid())
    {
        Online = true;
        MeetingUrl = meetingUrl;
    }

    private Location(string building, string street, short floor, string doorNumber, Faculty faculty) : base(Guid.NewGuid())
    {
        Online = false;
        Building = building;
        Street = street;
        Floor = floor;
        DoorNumber = doorNumber;
        FacultyId = faculty.Id;
    }


    public static Location AddOnline(string meetingUrl)
    {
        return new Location(meetingUrl);
    }

    public static Location Add(string building, string street, short floor, string doorNumber, Faculty faculty)
    {
        return new Location(building, street, floor, doorNumber, faculty);
    }

    public void Update(string building, string street, short floor, string doorNumber)
    {
        Building = building;
        Street = street;
        Floor = floor;
        DoorNumber = doorNumber;
    }

    public void UpdateOnline(string meetingUrl)
    {
        MeetingUrl = meetingUrl;
    }
}