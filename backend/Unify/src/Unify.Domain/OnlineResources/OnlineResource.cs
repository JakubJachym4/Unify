using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.Shared;

namespace Unify.Domain.OnlineResources;

public abstract class OnlineResource : Entity
{
    protected OnlineResource(){}
    protected OnlineResource(Guid id, Title title, Description description) : base(id)
    {
        Title = title;
        Description = description;
    }

    public Title Title { get; private set; }
    public Description Description { get; private set; }


    private List<Attachment> _files = new();
    public IReadOnlyCollection<Attachment> Files => _files;
    public void AddFile(Attachment attachment) => _files.Add(attachment);
    public void AddFiles(IEnumerable<Attachment> attachments) => _files.AddRange(attachments);
    public void RemoveFile(Attachment attachment) => _files.Remove(attachment);
}