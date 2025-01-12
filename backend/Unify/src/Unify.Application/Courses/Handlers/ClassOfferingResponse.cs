using Unify.Domain.UniversityClasses;

namespace Unify.Application.Courses.Handlers;

public record ClassOfferingResponse
{
    public static List<ClassOfferingResponse> FromClassOfferingList(List<ClassOffering> classOffering)
    {
        return classOffering.Select(c => new ClassOfferingResponse(c)).ToList();
    }
    
    public ClassOfferingResponse(Guid Id, string Name, string Description, DateTime StartDate, DateTime EndDate, Guid CourseId)
    {
        this.Id = Id;
        this.Name = Name;
        this.StartDate = StartDate;
        this.EndDate = EndDate;
        this.CourseId = CourseId;
    }
    
    public ClassOfferingResponse(ClassOffering classOffering)
    {
        Id = classOffering.Id;
        Name = classOffering.Name.Value;
        StartDate = classOffering.StartDate.ToDateTime(TimeOnly.MinValue);
        EndDate = classOffering.EndDate.ToDateTime(TimeOnly.MinValue);
        CourseId = classOffering.CourseId;
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public Guid CourseId { get; init; }
    
}