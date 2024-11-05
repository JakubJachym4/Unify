using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Messages.SendMessage;

public sealed record SendMessageCommand(
    string Title,
    string Content,
    ICollection<Guid> RecipientsIds,
    ICollection<IFormFile>? Attachments) : ICommand<Guid>;

