namespace Unify.Api.Controllers.Messages;

public sealed record ForwardMessageRequest
(
    Guid OriginalMessageId,
    ICollection<Guid> NewRecipientsIds
);
