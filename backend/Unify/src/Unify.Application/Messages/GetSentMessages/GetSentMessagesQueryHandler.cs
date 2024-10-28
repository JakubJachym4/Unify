using Dapper;
using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Data;
using Unify.Application.Abstractions.Files;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;

namespace Unify.Application.Messages.GetSentMessages;

internal sealed class GetSentMessagesQueryHandler : IQueryHandler<GetSentMessagesQuery, MessagesResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IMessageRepository _messageRepository;
    private readonly IFileConversionService _fileConversionService;

    public GetSentMessagesQueryHandler(IMessageRepository messageRepository, ISqlConnectionFactory sqlConnectionFactory, IFileConversionService fileConversionService)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _fileConversionService = fileConversionService;
        _messageRepository = messageRepository;
    }

    public async Task<Result<MessagesResponse>> Handle(GetSentMessagesQuery request, CancellationToken cancellationToken)
    {
        //using var connection = _sqlConnectionFactory.CreateConnection();

        // const string sql = @"
        //     SELECT
        //         id AS messageId,
        //         sender_id as senderId,
        //         title as title,
        //         content as content,
        //         created_on as createdOn
        //     FROM
        //         messages
        //     WHERE
        //         status = @status AND
        //         sender_id = @senderId";
        //
        // var messages = await connection.QueryMultipleAsync(
        //     sql,
        //     new
        //     {
        //         MessageStatus.NormalMessage,
        //         request.UserId
        //     });

        var messages = await _messageRepository.GetMultipleBySenderIdAsync(request.UserId, cancellationToken);

        var messageResponses = new List<MessageResponse>();
        foreach (var message in messages)
        {
            var files = new List<FileResponse>();

            if (message.Attachments.Count > 0)
            {
                var convertedFilesResult = await _fileConversionService.ConvertToFileResponses(message.Attachments.ToList());
                if (convertedFilesResult.Any(r => r.IsFailure))
                {
                    //TODO: create errors
                    return Result.Failure<MessagesResponse>(new Error("Error", "Could not get files from the database."));
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


        var response = new MessagesResponse(new List<MessageResponse>(messageResponses));

        return response;
    }
}