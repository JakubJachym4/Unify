using System.Net.Mail;
using Unify.Domain.Abstractions;
using Unify.Domain.Users;

namespace Unify.Domain.Messages;

public sealed class InformationMessage : Entity
{
    private InformationMessage() { }

    private readonly List<Attachment> _attachments = new();
    private List<Guid> _recipients = new();

    public Title Title { get; private set; }
    public TextContent Content { get; private set; }
    public DateOnly ExpirationDate { get; private set; }
    public IReadOnlyCollection<Guid> Recipients => _recipients;

}