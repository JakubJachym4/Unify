using Microsoft.AspNetCore.Http;

namespace Unify.Application.Messages.GetSentMessages;

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
    public MessageResponse(Guid messageId, Guid senderId, string title, string content, DateTime createdOn)
    {
        MessageId = messageId;
        SenderId = senderId;
        Title = title;
        Content = content;
        CreatedOn = createdOn;
    }

    public Guid MessageId { get; init; }
    public Guid SenderId { get; init; }
    public string Title { get; init; }
    public string Content { get; init; }
    public DateTime CreatedOn { get; init; }
    // public ICollection<Guid> RecipientsIds { get; init; } = new List<Guid>();
    // public ICollection<IFormFile> Files { get; init; } = new List<IFormFile>();
}