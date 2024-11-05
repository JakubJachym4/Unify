using Dapper;
using Microsoft.AspNetCore.Http;
using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Data;
using Unify.Application.Abstractions.Files;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Files;
using Unify.Application.Messages.Utils;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;

namespace Unify.Application.Messages.GetSentMessages;

internal sealed class GetSentMessagesQueryHandler : IQueryHandler<GetSentMessagesQuery, MessagesResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IMessageRepository _messageRepository;
    private readonly IFileConversionService _fileConversionService;
    private readonly IUserContext _userContext;

    public GetSentMessagesQueryHandler(IMessageRepository messageRepository, ISqlConnectionFactory sqlConnectionFactory, IFileConversionService fileConversionService, IUserContext userContext)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _fileConversionService = fileConversionService;
        _userContext = userContext;
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

        var messages = await _messageRepository.GetMultipleBySenderIdAsync(_userContext.UserId, cancellationToken);

        var converter = new MessageConverter(_fileConversionService);
        var messageResponsesResult = await converter.ConvertMessagesToResponses(messages);
        if (messageResponsesResult.IsFailure)
        {
            return Result.Failure<MessagesResponse>(messageResponsesResult.Error);
        }

        var response = new MessagesResponse(new List<MessageResponse>(messageResponsesResult.Value));

        return response;
    }

}