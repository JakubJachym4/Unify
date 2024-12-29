using Unify.Application.Files;

namespace Unify.Application.Messages;

public sealed class MessagesResponse
{
    public MessagesResponse(List<MessageResponse> messageResponses)
    {
        Messages = messageResponses;
    }

    public List<MessageResponse> Messages { get; init; }
}

public sealed class MessageResponse
{
    public MessageResponse(Guid messageId, Guid senderId, string title, string content, DateTime createdOn, List<Guid> recipientsIds, List<FileResponse> attachments, Guid? respondingToId, Guid? forwardedFromId)
    {
        MessageId = messageId;
        SenderId = senderId;
        Title = title;
        Content = content;
        CreatedOn = createdOn;
        RecipientsIds = recipientsIds;
        Attachments = attachments;
        RespondingToId = respondingToId;
        ForwardedFromId = forwardedFromId;
    }

    public Guid MessageId { get; init; }
    public Guid SenderId { get; init; }
    public string Title { get; init; }
    public string Content { get; init; }
    public DateTime CreatedOn { get; init; }
    public ICollection<Guid> RecipientsIds { get; init; }
    public ICollection<FileResponse> Attachments { get; init; }
    public Guid? RespondingToId { get; init; }
    public Guid? ForwardedFromId { get; init; }
}