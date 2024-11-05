using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Files;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Messages.Utils;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;

namespace Unify.Application.Messages.GetLastMessagesByNumber;

internal sealed class GetLastMessagesByNumberQueryHandler : IQueryHandler<GetLastMessagesByNumberQuery, MessagesResponse>
{
    private readonly IUserContext _userContext;
    private readonly IMessageRepository _messageRepository;
    private readonly IFileConversionService _fileConversionService;

    public GetLastMessagesByNumberQueryHandler(IMessageRepository messageRepository, IUserContext userContext, IFileConversionService fileConversionService)
    {
        _messageRepository = messageRepository;
        _userContext = userContext;
        _fileConversionService = fileConversionService;
    }

    public async Task<Result<MessagesResponse>> Handle(GetLastMessagesByNumberQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetLastMultipleBySenderAndNumberAsync(_userContext.UserId, request.NumberOfMessages, cancellationToken);

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