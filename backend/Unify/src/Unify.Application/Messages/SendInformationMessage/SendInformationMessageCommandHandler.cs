using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Clock;
using Unify.Application.Abstractions.Files;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Messages.Utils;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.Messages.InformationMessages;
using Unify.Domain.Shared;
using Unify.Domain.Users;

namespace Unify.Application.Messages.SendInformationMessage;

public sealed class SendInformationMessageCommandHandler : ICommandHandler<SendInformationMessageCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileConversionService _fileConversionService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IInformationMessageRepository _messageRepository;
    private readonly IUserContext _userContext;

    public SendInformationMessageCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IFileConversionService fileConversionService, IDateTimeProvider dateTimeProvider, IInformationMessageRepository messageRepository, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _fileConversionService = fileConversionService;
        _dateTimeProvider = dateTimeProvider;
        _messageRepository = messageRepository;
        _userContext = userContext;
    }


    public async Task<Result<Guid>> Handle(SendInformationMessageCommand request, CancellationToken cancellationToken)
    {
        var validator = new MessageRequestValidator(_userRepository, _fileConversionService);

        var userId = _userContext.UserId;

        var result = await validator.ValidateAsync(userId, request.RecipientsIds, request.Attachments, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        if (request.ExpirationDate < _dateTimeProvider.UtcNow)
        {
            return Result.Failure<Guid>(MessageErrors.WrongExpirationDate);
        }


        if (Enum.TryParse<InformationSeverityLevel>(request.Severity, out var severity) == false)
        {
            return Result.Failure<Guid>(MessageErrors.IncorrectSeverity);
        }

        //create and persist

        var message = InformationMessage.Send(result.Value.Sender,
            new Title(request.Title),
            new TextContent(request.Content),
            result.Value.Recipients,
            result.Value.Attachments,
            _dateTimeProvider.UtcNow,
            request.ExpirationDate,
            severity);

        _messageRepository.Add(message);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(message.Id);
    }
}