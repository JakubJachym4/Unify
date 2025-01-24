using Unify.Application.Files;
using Unify.Domain.OnlineResources;

namespace Unify.Application.OnlineResources.OfferingResources;

public record OfferingResourceResponse(Guid Id, string Title, string Description, List<FileResponse> Attachments)
{
    public static OfferingResourceResponse CreateFromOfferingResource(OfferingResource courseResource)
    {
        return new OfferingResourceResponse(courseResource.Id, courseResource.Title.Value, courseResource.Description.Value, FileResponse.FromManyAttachments(courseResource.Files.ToList()));
    }
}