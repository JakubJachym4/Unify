namespace Unify.Domain.Messages;

public sealed class MessageAttachments
{
    public Guid MessageId { get; set; }
    public Guid AttachmentId { get; set; }
}