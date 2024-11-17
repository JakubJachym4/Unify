using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Clock;
using Unify.Application.Abstractions.Files;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Messages.Utils;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.Shared;
using Unify.Domain.Users;

namespace Unify.Application.Messages.SendMessage;

public sealed class SendMessageCommandHandler : ICommandHandler<SendMessageCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileConversionService _fileConversionService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserContext _userContext;

    public SendMessageCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IFileConversionService fileConversionService, IDateTimeProvider dateTimeProvider, IMessageRepository messageRepository, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _fileConversionService = fileConversionService;
        _dateTimeProvider = dateTimeProvider;
        _messageRepository = messageRepository;
        _userContext = userContext;
    }

    public async Task<Result<Guid>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {

        var validator = new MessageRequestValidator(_userRepository, _fileConversionService);

        var userId = _userContext.UserId;

        var result = await validator.ValidateAsync(userId, request.RecipientsIds, request.Attachments, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        //create and persist

        var message = Message.Send(result.Value.Sender,
            new Title(request.Title),
            new TextContent(request.Content),
            result.Value.Recipients,
            result.Value.Attachments,
            _dateTimeProvider.UtcNow);

        _messageRepository.Add(message);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(message.Id);
    }

}