using Unify.Domain.Abstractions;

namespace Unify.Domain.OnlineResources;

public sealed class Attachment : Entity
{
    public Attachment(string fileName, byte[] data)
    {
        FileName = fileName;
        Data = data;
    }

    public string FileName { get; private set; }
    public byte[] Data { get; private set; }

    public string Extension => Path.GetExtension(FileName);

}