namespace Unify.Api.Controllers.Messages;

public record SendMessageRequest
(
    string Title,
    string Content,
    ICollection<Guid> RecipientsIds,
    ICollection<IFormFile>? Attachments);