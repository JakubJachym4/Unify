using Unify.Domain.Abstractions;

namespace Unify.Domain.OnlineResources;

public static class AttachmentsErrors
{
    public static Error NotFound = new(
        "Attachment.NotFound",
        "The attachment was not found.");

    public static Error Conversion = new(
        "Attachment.Conversion",
        "There was a problem with file conversion.");
}