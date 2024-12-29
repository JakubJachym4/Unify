namespace Unify.Api.Controllers.Messages;

public sealed record ReplyToMessageRequest
(
    Guid RespondingToId,
    string Title,
    string Content,
    ICollection<Guid> RecipientsIds,
    ICollection<IFormFile>? Attachments);