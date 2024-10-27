using Unify.Domain.Abstractions;
using Unify.Domain.Messages.Events;
using Unify.Domain.Users;

namespace Unify.Domain.Messages;

public sealed class Message : Entity
{
    private Message() { }

    private Message(Guid id, Title title, Guid senderId, TextContent content, DateTime createdOn, Guid? respondingToMessageId = null, Guid? forwardedFromMessageId = null) : base(id)
    {
        Title = title;
        SenderId = senderId;
        Content = content;
        CreatedOn = createdOn;
        RespondingToMessageId = respondingToMessageId;
        ForwardedFromMessageId = forwardedFromMessageId;
        Status = MessageStatus.NormalMessage;
    }


    private readonly List<Attachment> _attachments = new();
    private List<User> _recipients = new();

    public Title Title { get; private set; }
    public Guid SenderId { get; private set; }

    public TextContent Content { get; private set; }
    public Guid? RespondingToMessageId { get; private set; }
    public Guid? ForwardedFromMessageId { get; private set; }
    public MessageStatus Status { get; private set; }
    public IReadOnlyCollection<Attachment> Attachments => _attachments;
    public IReadOnlyCollection<User> Recipients => _recipients;

    public DateTime CreatedOn { get; private set; }

    public static Message Send(
        User sender,
        Title title,
        TextContent content,
        List<User> recipients,
        List<Attachment> attachments,
        DateTime createdOn
    )
    {
        var message = new Message(
            Guid.NewGuid(),
            title,
            sender.Id,
            content,
            createdOn);

        message._attachments.AddRange(attachments);
        message._recipients.AddRange(recipients);

        message.RaiseDomainEvent(new MessageSendDomainEvent(sender.Id, message.Recipients.Select(r => r.Id).ToList()));

        return message;
    }

    public static Message Respond(User responder,
        Title title,
        TextContent content,
        Message messageRespondingTo,
        List<User> recipients,
        List<Attachment> attachments,
        DateTime createdOn)
    {
        var message = new Message(
            Guid.NewGuid(),
            title,
            responder.Id,
            content,
            createdOn,
            messageRespondingTo.Id);

        message._attachments.AddRange(attachments);
        message._recipients.AddRange(recipients);

        message.RaiseDomainEvent(new MessageSendDomainEvent(responder.Id, message.Recipients.Select(r => r.Id).ToList()));

        return message;
    }

    public static Message Forward(User sender, Message message, List<User> recipients, DateTime createdOn)
    {
        var forwardedMessage = new Message(
            Guid.NewGuid(),
            message.Title,
            sender.Id,
            message.Content,
            createdOn,
            message.RespondingToMessageId,
            message.Id);

        forwardedMessage._attachments.AddRange(message.Attachments);

        forwardedMessage._recipients.AddRange(recipients);
        forwardedMessage.Status = MessageStatus.Forwarded;

        forwardedMessage.RaiseDomainEvent(new MessageSendDomainEvent(sender.Id, message.Recipients.Select(r => r.Id).ToList()));

        return forwardedMessage;
    }



}