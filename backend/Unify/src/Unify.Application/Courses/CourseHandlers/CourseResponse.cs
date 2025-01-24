using Unify.Domain.UniversityCore;

namespace Unify.Application.Courses.CourseHandlers;

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