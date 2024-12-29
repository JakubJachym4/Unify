namespace Unify.Api.Controllers.Messages.InformationMessages;

public record SendInformationMessageRequest(
    string Title,
    string Content,
    string Severity,
    DateTime ExpirationDate,
    ICollection<Guid> RecipientsIds,
    ICollection<IFormFile>? Attachments);