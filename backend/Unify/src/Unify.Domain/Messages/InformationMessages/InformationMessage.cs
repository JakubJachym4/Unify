using Unify.Domain.Abstractions;
using Unify.Domain.Messages.Events;
using Unify.Domain.OnlineResources;
using Unify.Domain.Shared;
using Unify.Domain.Users;

namespace Unify.Domain.Messages.InformationMessages;

public sealed class InformationMessage : Entity
{
    private InformationMessage() { }

    private InformationMessage(Guid messageId, Guid senderId, Title title, TextContent content, DateTime expirationDate, DateTime createdOn, InformationSeverityLevel severityLevel) : base(messageId)
    {
        SenderId = senderId;
        Title = title;
        Content = content;
        ExpirationDate = expirationDate;
        CreatedOn = createdOn;
        SeverityLevel = severityLevel;
    }

    private readonly List<Attachment> _attachments = new();
    private List<User> _recipients = new();

    public Guid SenderId { get; private set; }

    public Title Title { get; private set; }
    public TextContent Content { get; private set; }
    public DateTime ExpirationDate { get; private set; }
    public DateTime CreatedOn { get; private set; }

    public InformationSeverityLevel SeverityLevel { get; private set; }
    public IReadOnlyCollection<Attachment> Attachments => _attachments;
    public IReadOnlyCollection<User> Recipients => _recipients;

    public static InformationMessage Send(
        User sender,
        Title title,
        TextContent content,
        List<User> recipients,
        List<Attachment> attachments,
        DateTime createdOn,
        DateTime expirationDate,
        InformationSeverityLevel severityLevel
    )
    {
        var message = new InformationMessage(
            Guid.NewGuid(),
            sender.Id,
            title,
            content,
            expirationDate,
            createdOn,
            severityLevel);

        message._attachments.AddRange(attachments);
        message._recipients.AddRange(recipients);

        message.RaiseDomainEvent(new MessageSendDomainEvent(sender.Id, message.Recipients.Select(r => r.Id).ToList()));

        return message;
    }

}