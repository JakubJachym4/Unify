using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Clock;
using Unify.Application.Abstractions.Data;
using Unify.Application.Abstractions.Files;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Messages.Utils;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages.InformationMessages;

namespace Unify.Application.Messages.GetNonExpiredInformationMessages;

internal sealed class GetNonExpiredInformationMessagesQueryHandler : IQueryHandler<GetNonExpiredInformationMessagesQuery, InformationMessagesResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IInformationMessageRepository _messageRepository;
    private readonly IFileConversionService _fileConversionService;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetNonExpiredInformationMessagesQueryHandler(ISqlConnectionFactory sqlConnectionFactory, IInformationMessageRepository messageRepository, IFileConversionService fileConversionService, IUserContext userContext, IDateTimeProvider dateTimeProvider)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _messageRepository = messageRepository;
        _fileConversionService = fileConversionService;
        _userContext = userContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<InformationMessagesResponse>> Handle(GetNonExpiredInformationMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetNonExpired(_dateTimeProvider.UtcNow, cancellationToken);

        var converter = new MessageConverter(_fileConversionService);
        var messageResponsesResult = await converter.ConvertMessagesToResponses(messages);
        if (messageResponsesResult.IsFailure)
        {
            return Result.Failure<InformationMessagesResponse>(messageResponsesResult.Error);
        }

        var response = new InformationMessagesResponse(new List<InformationMessageResponse>(messageResponsesResult.Value));

        return response;
    }
}