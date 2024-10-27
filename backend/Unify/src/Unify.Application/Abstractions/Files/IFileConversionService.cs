using Microsoft.AspNetCore.Http;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;

namespace Unify.Application.Abstractions.Files;

public interface IFileConversionService
{
    Task<Result<Attachment>> ConvertToAttachment(IFormFile file);
    Task<ICollection<Result<Attachment>>> ConvertToAttachments(ICollection<IFormFile> files);
}