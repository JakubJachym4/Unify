using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.Messages.ForwardMessage;

public record ForwardMessageCommand(
    Guid OriginalMessageId,
    ICollection<Guid> NewRecipientsIds) : ICommand<Guid>;
