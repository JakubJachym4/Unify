using Unify.Application.Files;
using Unify.Domain.OnlineResources;

namespace Unify.Application.Courses.CourseHandlers;

public record CourseResourceResponse(Guid Id, string Title, string Description, List<FileResponse> Attachments)
{
    public static CourseResourceResponse CreateFromCourseResource(CourseResource courseResource)
    {
        return new CourseResourceResponse(courseResource.Id, courseResource.Title.Value, courseResource.Description.Value, FileResponse.FromManyAttachments(courseResource.Files.ToList()));
    }
}