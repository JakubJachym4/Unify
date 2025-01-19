using Unify.Domain.UniversityClasses;

namespace Unify.Application.Courses.CourseHandlers;

public record ClassOfferingResponse(Guid Id, string Name, Guid CourseId, DateTime StartDate, DateTime EndDate, Guid LecturerId, Guid StudentGroupId, int MaxStudentsCount)
{
    public static List<ClassOfferingResponse> FromClassOfferingList(List<ClassOffering> classOffering)
    {
        return classOffering.Select(FromClassOffering).ToList();
    }

    public ClassOfferingResponse(ClassOffering classOffering) :
        this(
        classOffering.Id,
        classOffering.Name.Value,
        classOffering.CourseId,
        classOffering.StartDate.ToDateTime(TimeOnly.MinValue),
        classOffering.EndDate.ToDateTime(TimeOnly.MinValue),
        classOffering.LecturerId,
        classOffering.StudentGroupId,
        classOffering.MaxStudentsCount)
    {

    }

    public static ClassOfferingResponse FromClassOffering(ClassOffering classOffering)
    {
        return new ClassOfferingResponse(classOffering);
    }

}