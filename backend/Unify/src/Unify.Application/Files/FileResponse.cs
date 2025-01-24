using Attachment = Unify.Domain.OnlineResources.Attachment;

namespace Unify.Application.Files;

public sealed class FileResponse
{
    public FileResponse(string fileName, string contentType, string data)
    {
        FileName = fileName;
        ContentType = contentType;
        Data = data;
    }

    public string FileName { get; init; }
    public string ContentType { get; init; }
    public string Data { get; init; }


    public static FileResponse? FromAttachment(Attachment attachment)
    {
        if (attachment.Data.Length == 0)
        {
            return null;
        }

        var base64Data = Convert.ToBase64String(attachment.Data);

        var contentType = attachment.Extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };

        return new FileResponse(attachment.FileName, contentType, base64Data);
    }

    public static List<FileResponse> FromManyAttachments(ICollection<Attachment> attachments)
    {
        var files = new List<FileResponse>();
        foreach (var attachment in attachments)
        {
            var file = FromAttachment(attachment);
            if(file != null)
            {
                files.Add(file);
            }
        }
        return files;
    }

}