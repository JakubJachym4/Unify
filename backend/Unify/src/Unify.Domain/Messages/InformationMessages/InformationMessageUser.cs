namespace Unify.Domain.Messages.InformationMessages;

public sealed class InformationMessageUser
{
    public Guid InformationMessageId { get; set; }
    public Guid UserId { get; set; }
}