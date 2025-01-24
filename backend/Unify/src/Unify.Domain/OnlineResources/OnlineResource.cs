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

    public Title Title { get; protected set; }
    public Description Description { get; protected set; }

    public void Update(Title title, Description description)
    {
        Title = title;
        Description = description;
    }


    private List<Attachment> _files = new();
    public IReadOnlyCollection<Attachment> Files => _files;
    public void AddFile(Attachment attachment) => _files.Add(attachment);
    public void AddFiles(IEnumerable<Attachment> attachments) => _files.AddRange(attachments);
    public void RemoveFile(Attachment attachment) => _files.Remove(attachment);
    public void ClearFiles() => _files.Clear();
}