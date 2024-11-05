using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Messages.ReplyToMessage;

public sealed record ReplyToMessageCommand(
    Guid MessageId,
    string Title,
    string Content,
    ICollection<Guid> RecipientsIds,
    ICollection<IFormFile>? Attachments) : ICommand<Guid>;