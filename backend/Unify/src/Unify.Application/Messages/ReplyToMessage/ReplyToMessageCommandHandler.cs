using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Clock;
using Unify.Application.Abstractions.Files;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Messages.Utils;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.Users;

namespace Unify.Application.Messages.ReplyToMessage;

public sealed class ReplyToMessageCommandHandler : ICommandHandler<ReplyToMessageCommand, Guid>
{
    private readonly IUserContext _userContext;
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IFileConversionService _fileConversionService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;


    public ReplyToMessageCommandHandler(IUserContext userContext, IMessageRepository messageRepository, IFileConversionService fileConversionService, IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _userContext = userContext;
        _messageRepository = messageRepository;
        _fileConversionService = fileConversionService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<Result<Guid>> Handle(ReplyToMessageCommand request, CancellationToken cancellationToken)
    {
        var validator = new MessageRequestValidator(_userRepository, _fileConversionService);

        var userId = _userContext.UserId;

        var messageRespondingTo = await _messageRepository.GetByIdAsync(request.MessageId, cancellationToken);

        if (messageRespondingTo == null)
        {
            return Result.Failure<Guid>(MessageErrors.NotFound(request.MessageId));
        }

        var result = await validator.ValidateAsync(userId, request.RecipientsIds, request.Attachments, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        //create and persist

        var message = Message.Respond(
            result.Value.Sender,
            new Title(request.Title),
            new TextContent(request.Content),
            messageRespondingTo,
            result.Value.Recipients,
            result.Value.Attachments,
            _dateTimeProvider.UtcNow);

        _messageRepository.Add(message);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(message.Id);
    }
}