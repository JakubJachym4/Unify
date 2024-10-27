using Unify.Domain.Users;

namespace Unify.Domain.Messages;

public sealed class MessageUser
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
}