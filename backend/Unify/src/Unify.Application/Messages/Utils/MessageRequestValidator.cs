using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Files;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.OnlineResources;
using Unify.Domain.Users;

namespace Unify.Application.Messages.Utils;

internal class MessageRequestValidator
{
    private readonly IUserRepository _userRepository;
    private readonly IFileConversionService _fileConversionService;

    public MessageRequestValidator(IUserRepository userRepository, IFileConversionService fileConversionService)
    {
        _userRepository = userRepository;
        _fileConversionService = fileConversionService;
    }

    public async Task<Result<MessageRequestValidatorResult>> ValidateAsync(Guid userId, ICollection<Guid> recipientsIds, ICollection<IFormFile>? attachments, CancellationToken cancellationToken)
    {
        // Validate sender
        var sender = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (sender is null)
        {
            return Result.Failure<MessageRequestValidatorResult>(MessageErrors.SenderNotFound);
        }

        // Validate recipients
        var recipients = await _userRepository.GetManyByIdAsync(recipientsIds, cancellationToken);
        if (recipients.Count == 0)
        {
            return Result.Failure<MessageRequestValidatorResult>(MessageErrors.RecipientNotFound(recipientsIds.First()));
        }

        // Validate attachments
        var attachmentsResult = new List<Attachment>();
        if (attachments != null && attachments.Any())
        {
            var recipientsNotFound = recipients.Where(r => !recipientsIds.Contains(r.Id)).ToList();
            if (recipientsNotFound.Any())
            {
                return Result.Failure<MessageRequestValidatorResult>(MessageErrors.RecipientNotFound(recipientsNotFound.First().Id));
            }

            var attachmentResults = await _fileConversionService.ConvertToAttachments(attachments);
            if (attachmentResults.Any(result => result.IsFailure))
            {
                var firstError = attachmentResults.First(result => result.IsFailure);
                return Result.Failure<MessageRequestValidatorResult>(firstError.Error);
            }
            attachmentsResult.AddRange(attachmentResults.Select(a => a.Value));
        }

        return Result.Success(new MessageRequestValidatorResult(sender, recipients.ToList(), attachmentsResult));
    }
}

internal sealed record MessageRequestValidatorResult(
    User Sender,
    List<User> Recipients,
    List<Attachment> Attachments);
