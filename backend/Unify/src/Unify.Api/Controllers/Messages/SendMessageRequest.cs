namespace Unify.Api.Controllers.Messages;

public sealed record SendMessageRequest
(
    Guid SenderId,
    string Title,
    string Content,
    ICollection<Guid> RecipientsIds,
    ICollection<IFormFile>? Attachments);