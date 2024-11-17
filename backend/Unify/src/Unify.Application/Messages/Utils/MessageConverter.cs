using Unify.Application.Abstractions.Files;
using Unify.Application.Files;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.Messages.InformationMessages;
using Unify.Domain.OnlineResources;

namespace Unify.Application.Messages.Utils;

internal sealed class MessageConverter
{
    private readonly IFileConversionService _fileConversionService;

    public MessageConverter(IFileConversionService fileConversionService)
    {
        _fileConversionService = fileConversionService;
    }

    internal async Task<Result<List<MessageResponse>>> ConvertMessagesToResponses(ICollection<Message> messages)
    {
        var messageResponses = new List<MessageResponse>();
        foreach (var message in messages)
        {
            var files = new List<FileResponse>();

            if (message.Attachments.Count > 0)
            {
                var convertedFilesResult = await _fileConversionService.ConvertToFileResponses(message.Attachments.ToList());
                if (convertedFilesResult.Any(r => r.IsFailure))
                {
                    return Result.Failure<List<MessageResponse>>(AttachmentsErrors.Conversion);
                }
                files.AddRange(convertedFilesResult.Select(r => r.Value));
            }

            messageResponses.Add(
                new MessageResponse(
                    message.Id,
                    message.SenderId,
                    message.Title.Value,
                    message.Content.Value,
                    message.CreatedOn,
                    message.Recipients.Select(r => r.Id).ToList(),
                    files.ToList()
                ));
        }
        return messageResponses;
    }

        internal async Task<Result<List<InformationMessageResponse>>> ConvertMessagesToResponses(ICollection<InformationMessage> informationMessages)
        {
            var messageResponses = new List<InformationMessageResponse>();
            foreach (var message in informationMessages)
            {
                var files = new List<FileResponse>();

                if (message.Attachments.Count > 0)
                {
                    var convertedFilesResult = await _fileConversionService.ConvertToFileResponses(message.Attachments.ToList());
                    if (convertedFilesResult.Any(r => r.IsFailure))
                    {
                        return Result.Failure<List<InformationMessageResponse>>(AttachmentsErrors.Conversion);
                    }
                    files.AddRange(convertedFilesResult.Select(r => r.Value));
                }

                messageResponses.Add(
                    new InformationMessageResponse(
                        message.Id,
                        message.SenderId,
                        message.Title.Value,
                        message.Content.Value,
                        message.CreatedOn,
                        message.ExpirationDate,
                        message.SeverityLevel.ToString(),
                        message.Recipients.Select(r => r.Id).ToList(),
                        files.ToList()
                    ));
            }

        return messageResponses;
    }
}