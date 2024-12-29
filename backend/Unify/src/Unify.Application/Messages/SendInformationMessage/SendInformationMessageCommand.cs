using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Messages.SendInformationMessage;

public record SendInformationMessageCommand(
    string Title,
    string Content,
    string Severity,
    DateTime ExpirationDate,
    ICollection<Guid> RecipientsIds,
    ICollection<IFormFile>? Attachments
) : ICommand<Guid>;