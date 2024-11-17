using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Files;
using Unify.Application.Files;
using Unify.Application.Messages.GetSentMessages;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.OnlineResources;

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

    public async Task<Result<FileResponse>> ConvertToFileResponse(Attachment attachment)
    {
        if (attachment.Data.Length == 0)
        {
            return Result.Failure<FileResponse>(Error.NullValue);
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

    public async Task<ICollection<Result<FileResponse>>> ConvertToFileResponses(ICollection<Attachment> attachments)
    {
        var files = new List<Result<FileResponse>>();
        foreach (var attachment in attachments)
        {
            files.Add(await ConvertToFileResponse(attachment));
        }
        return files;
    }
}