using Unify.Application.Files;
using Unify.Domain.Messages.InformationMessages;

namespace Unify.Application.Messages;

public sealed class InformationMessagesResponse
{
    public List<InformationMessageResponse> InformationMessageResponses { get; set; }

    public InformationMessagesResponse(List<InformationMessageResponse> informationMessageResponses)
    {
        InformationMessageResponses = informationMessageResponses;
    }
}

public sealed class InformationMessageResponse
{
    public InformationMessageResponse(Guid informationMessageId, Guid senderId, string title, string content, DateTime createdOn, DateTime expirationDate, string informationSeverityLevel, ICollection<Guid> recipientsIds, ICollection<FileResponse> attachments)
    {
        InformationMessageId = informationMessageId;
        SenderId = senderId;
        Title = title;
        Content = content;
        CreatedOn = createdOn;
        ExpirationDate = expirationDate;
        InformationSeverityLevel = informationSeverityLevel;
        RecipientsIds = recipientsIds;
        Attachments = attachments;
    }


    public Guid InformationMessageId { get; init; }
    public Guid SenderId { get; init; }
    public string Title { get; init; }
    public string Content { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime ExpirationDate { get; init; }
    public string InformationSeverityLevel { get; init; }
    public ICollection<Guid> RecipientsIds { get; init; }
    public ICollection<FileResponse> Attachments { get; init; }
}