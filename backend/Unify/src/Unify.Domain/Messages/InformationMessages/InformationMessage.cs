using Unify.Domain.Abstractions;
using Unify.Domain.Users;

namespace Unify.Domain.Messages.InformationMessages;

public sealed class InformationMessage : Entity
{
    private InformationMessage() { }

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

}