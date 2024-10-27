using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Files;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;

namespace Unify.Infrastructure.FileUpload;

public sealed class FileConverter : IFileConversionService
{
    public async Task<Result<Attachment>> ConvertToAttachment(IFormFile file)
    {
        if (file.Length == 0)
        {
            return Result.Failure<Attachment>(Error.NullValue);
        }

        byte[] byteData;

        try
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            byteData = stream.ToArray();
        }
        catch (Exception)
        {
            return Result.Failure<Attachment>(new Error("FileConverter.Exception", "There was an exception when trying to access file."));
        }
        return new Attachment(file.FileName, byteData);
    }

    public async Task<ICollection<Result<Attachment>>> ConvertToAttachments(ICollection<IFormFile> files)
    {
        var attachments = new List<Result<Attachment>>();
        foreach (var file in files)
        {
            attachments.Add(await ConvertToAttachment(file));
        }
        return attachments;
    }
}