﻿using Microsoft.AspNetCore.Http;
using Unify.Application.Files;
using Unify.Application.Messages.GetSentMessages;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.OnlineResources;

namespace Unify.Application.Abstractions.Files;

public interface IFileConversionService
{
    Task<Result<Attachment>> ConvertToAttachment(IFormFile file);
    Task<ICollection<Result<Attachment>>> ConvertToAttachments(ICollection<IFormFile> files);

    Task<ICollection<Result<FileResponse>>> ConvertToFileResponses(ICollection<Attachment> attachments);
    Task<Result<FileResponse>> ConvertToFileResponse(Attachment attachment);
}