using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Clock;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.Users;
using Unify.Domain.Users.Extensions;

namespace Unify.Application.Messages.ForwardMessage;

internal sealed class ForwardMessageCommandHandler : ICommandHandler<ForwardMessageCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ForwardMessageCommandHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork, IUserRepository userRepository, IUserContext userContext, IDateTimeProvider dateTimeProvider)
    {
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _userContext = userContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> Handle(ForwardMessageCommand request, CancellationToken cancellationToken)
    {
        var originalMessage = await _messageRepository.GetByIdAsync(request.OriginalMessageId, cancellationToken);

        if (originalMessage is null)
        {
            return Result.Failure<Guid>(MessageErrors.NotFound(request.OriginalMessageId));
        }
        var sender = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);

        var recipients = await _userRepository.GetManyByIdAsync(request.NewRecipientsIds, cancellationToken);

        if(!recipients.Any() || !recipients.AllFound(request.NewRecipientsIds))
        {
            return Result.Failure<Guid>(UserErrors.NotFound(request.NewRecipientsIds.FirstOrDefault()));
        }

        var message = Message.Forward(sender!, originalMessage, recipients.ToList(), _dateTimeProvider.UtcNow);

        _messageRepository.Add(message);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(message.Id);
    }
}